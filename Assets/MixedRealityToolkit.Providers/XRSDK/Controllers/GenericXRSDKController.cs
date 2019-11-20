﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK.Input
{
    [MixedRealityController(
        SupportedControllerType.GenericUnity,
        new[] { Handedness.Left, Handedness.Right },
        flags: MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings)]
    public class GenericXRSDKController : BaseController
    {
        public GenericXRSDKController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// The current pose of this XR SDK controller.
        /// </summary>
        protected MixedRealityPose CurrentControllerPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// The previous pose of this XR SDK controller.
        /// </summary>
        protected MixedRealityPose LastControllerPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// The current position of this XR SDK controller.
        /// </summary>
        protected Vector3 CurrentControllerPosition = Vector3.zero;

        /// <summary>
        /// The current rotation of this XR SDK controller.
        /// </summary>
        protected Quaternion CurrentControllerRotation = Quaternion.identity;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        /// <inheritdoc />
        public virtual void UpdateController(InputDevice inputDevice)
        {
            if (!Enabled) { return; }

            if (Interactions == null)
            {
                Debug.LogError($"No interaction configuration for {GetType().Name}");
                Enabled = false;
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].AxisType)
                {
                    case AxisType.None:
                        break;
                    case AxisType.Digital:
                        UpdateButtonData(Interactions[i], inputDevice);
                        break;
                    case AxisType.SingleAxis:
                        UpdateSingleAxisData(Interactions[i], inputDevice);
                        break;
                    case AxisType.DualAxis:
                        UpdateDualAxisData(Interactions[i], inputDevice);
                        break;
                    case AxisType.SixDof:
                        UpdatePoseData(Interactions[i], inputDevice);
                        break;
                    default:
                        Debug.LogError($"Input [{Interactions[i].InputType}] is not handled for this controller [{GetType().Name}]");
                        break;
                }
            }

            var lastState = TrackingState;

            LastControllerPose = CurrentControllerPose;

            // The source is either a hand or a controller that supports pointing.
            // We can now check for position and rotation.
            IsPositionAvailable = state.TryGetPosition(out CurrentControllerPosition);
            IsPositionApproximate = false;

            IsRotationAvailable = state.TryGetRotation(out CurrentControllerRotation);

            // Devices are considered tracked if we receive position OR rotation data from the sensors.
            TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;

            CurrentControllerPosition = MixedRealityPlayspace.TransformPoint(CurrentControllerPosition);
            CurrentControllerRotation = MixedRealityPlayspace.Rotation * CurrentControllerRotation;

            CurrentControllerPose.Position = CurrentControllerPosition;
            CurrentControllerPose.Rotation = CurrentControllerRotation;

            // Raise input system events if it is enabled.
            if (lastState != TrackingState)
            {
                CoreServices.InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            if (TrackingState == TrackingState.Tracked && LastControllerPose != CurrentControllerPose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    CoreServices.InputSystem?.RaiseSourcePoseChanged(InputSource, this, CurrentControllerPose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    CoreServices.InputSystem?.RaiseSourcePositionChanged(InputSource, this, CurrentControllerPosition);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    CoreServices.InputSystem?.RaiseSourceRotationChanged(InputSource, this, CurrentControllerRotation);
                }
            }
        }

        /// <summary>
        /// Update an interaction bool data type from a bool input
        /// </summary>
        /// <remarks>
        /// Raises an Input System "Input Down" event when the key is down, and raises an "Input Up" when it is released (e.g. a Button)
        /// </remarks>
        protected virtual void UpdateButtonData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            if (interactionMapping.InputType == DeviceInputType.TriggerTouch
                && inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerData))
            {
                interactionMapping.BoolData = !Mathf.Approximately(triggerData, 0.0f);
            }
            else
            {
                InputFeatureUsage<bool> buttonUsage;

                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.Select:
                        buttonUsage = CommonUsages.triggerButton;
                        break;
                    case DeviceInputType.TouchpadTouch:
                        buttonUsage = CommonUsages.primaryTouch;
                        break;
                    case DeviceInputType.TouchpadPress:
                        buttonUsage = CommonUsages.primaryButton;
                        break;
                    case DeviceInputType.Menu:
                        buttonUsage = CommonUsages.menuButton;
                        break;
                    case DeviceInputType.ThumbStickPress:
                        buttonUsage = CommonUsages.secondaryButton;
                        break;
                    default:
                        return;
                }

                if (inputDevice.TryGetFeatureValue(buttonUsage, out bool buttonPressed))
                {
                    interactionMapping.BoolData = buttonPressed;
                }
            }

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system event if it's enabled
                if (interactionMapping.BoolData)
                {
                    CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }
        }

        /// <summary>
        /// Update an interaction float data type from a SingleAxis (float) input
        /// </summary>
        /// <remarks>
        /// Raises a FloatInputChanged event when the float data changes
        /// </remarks>
        protected virtual void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);

            // Update the interaction data source
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TriggerPress:
                    if (inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool buttonPressed))
                    {
                        interactionMapping.BoolData = buttonPressed;
                    }

                    // If our bool value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system event if it's enabled
                        if (interactionMapping.BoolData)
                        {
                            CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }

                    if (inputDevice.TryGetFeatureValue(CommonUsages.grip, out float buttonData))
                    {
                        interactionMapping.FloatData = buttonData;
                    }
                    break;
                case DeviceInputType.Trigger:
                    if (inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerData))
                    {
                        interactionMapping.FloatData = triggerData;
                    }
                    break;
                default:
                    return;
            }

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system event if it's enabled
                CoreServices.InputSystem?.RaiseFloatInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.FloatData);
            }
        }

        /// <summary>
        /// Update the touchpad / thumbstick input from the device
        /// </summary>
        protected virtual void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

            InputFeatureUsage<Vector2> axisUsage;

            // Update the interaction data source
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStick:
                    axisUsage = CommonUsages.secondary2DAxis;
                    break;
                case DeviceInputType.Touchpad:
                    axisUsage = CommonUsages.primary2DAxis;
                    break;
                default:
                    return;
            }

            if (inputDevice.TryGetFeatureValue(axisUsage, out Vector2 axisData))
            {
                // Update the interaction data source
                interactionMapping.Vector2Data = axisData;
            }

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system event if it's enabled
                CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.Vector2Data);
            }
        }

        /// <summary>
        /// Update spatial grip data.
        /// </summary>
        protected virtual void UpdatePoseData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);

            InputFeatureUsage<Vector3> positionUsage;
            InputFeatureUsage<Quaternion> rotationUsage;

            // Update the interaction data source
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.SpatialGrip:
                    positionUsage = CommonUsages.devicePosition;
                    rotationUsage = CommonUsages.deviceRotation;
                    break;
                default:
                    return;
            }

            if (inputDevice.TryGetFeatureValue(positionUsage, out Vector3 position))
            {
                CurrentControllerPose.Position = position;
            }

            if (inputDevice.TryGetFeatureValue(rotationUsage, out Quaternion rotation))
            {
                CurrentControllerPose.Rotation = rotation;
            }

            interactionMapping.PoseData = CurrentControllerPose;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system event if it's enabled
                CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
            }
        }
    }
}