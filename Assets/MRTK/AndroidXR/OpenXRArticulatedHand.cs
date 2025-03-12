// Copyright (c) Microsoft Corporation.
// Copyright (c) Qualcomm.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.OpenXR;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK.AndroidXR
{
    /// <summary>
    /// OpenXR + XR SDK implementation of
    /// <see href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_hand_interaction">XR_EXT_hand_interaction</see> and
    /// <see href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_hand_tracking">XR_EXT_hand_tracking</see>.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.ArticulatedHand,
        new[] { Handedness.Left, Handedness.Right })]
    public class OpenXRArticulatedHand : MicrosoftArticulatedHand
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OpenXRArticulatedHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            handDefinition = Definition as ArticulatedHandDefinition;
            handMeshProvider = controllerHandedness == Handedness.Left ? AndroidXRHandMeshProvider.Left : AndroidXRHandMeshProvider.Right;
            handMeshProvider?.SetInputSource(inputSource);
            handJointProvider = new UnityXRHandJointProvider(controllerHandedness);
        }

        private readonly ArticulatedHandDefinition handDefinition;
        private readonly AndroidXRHandMeshProvider handMeshProvider;
        private readonly UnityXRHandJointProvider handJointProvider;

        private static readonly InputFeatureUsage<bool> PointerActivated = new InputFeatureUsage<bool>("PointerActivated");
        private static readonly InputFeatureUsage<float> PointerActivateValue = new InputFeatureUsage<float>("PointerActivateValue");
        private static readonly InputFeatureUsage<bool> GraspFirm = new InputFeatureUsage<bool>("GraspFirm");
        private static readonly InputFeatureUsage<float> GraspValue = new InputFeatureUsage<float>("GraspValue");

        public override void UpdateController(InputDevice inputDevice)
        {
            if (!Enabled) { return; }

            if (Interactions == null)
            {
                Debug.LogError($"No interaction configuration for {GetType().Name}");
                Enabled = false;
            }

            base.UpdateController(inputDevice);

            UpdateHandData();
        }

        private static readonly ProfilerMarker UpdateSingleAxisDataPerfMarker = new ProfilerMarker("[MRTK] OpenXRArticulatedHand.UpdateSingleAxisData");

        /// <inheritdoc />
        protected override void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdateSingleAxisDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);
                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.TriggerPress:
                        if (inputDevice.TryGetFeatureValue(PointerActivated, out bool pointerActivated))
                        {
                            interactionMapping.BoolData = pointerActivated;
                        }
                        break;
                    case DeviceInputType.GripPress:
                        if (inputDevice.TryGetFeatureValue(GraspFirm, out bool graspFirm))
                        {
                            interactionMapping.BoolData = graspFirm;
                        }
                        break;
                    default:
                        base.UpdateSingleAxisData(interactionMapping, inputDevice);
                        return;
                }

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise bool input system event if it's available
                    if (interactionMapping.BoolData)
                    {
                        CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                    else
                    {
                        CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                }

                // Next handle updating the float values
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.Trigger:
                        if (inputDevice.TryGetFeatureValue(PointerActivateValue, out float pointerActivateValue))
                        {
                            interactionMapping.FloatData = pointerActivateValue;
                        }
                        break;
                    case DeviceInputType.Grip:
                        if (inputDevice.TryGetFeatureValue(GraspValue, out float graspValue))
                        {
                            interactionMapping.FloatData = graspValue;
                        }
                        break;
                    default:
                        return;
                }

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise float input system event if it's enabled
                    CoreServices.InputSystem?.RaiseFloatInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.FloatData);
                }
            }
        }

        private static readonly ProfilerMarker UpdateButtonDataPerfMarker = new ProfilerMarker("[MRTK] OpenXRArticulatedHand.UpdateButtonData");

        /// <inheritdoc />
        protected override void UpdateButtonData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdateButtonDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.Select:
                        if (inputDevice.TryGetFeatureValue(PointerActivated, out bool pointerActivated))
                        {
                            interactionMapping.BoolData = pointerActivated;
                        }
                        else
                        {
                            base.UpdateButtonData(interactionMapping, inputDevice);
                            return;
                        }
                        break;
                    default:
                        base.UpdateButtonData(interactionMapping, inputDevice);
                        return;
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
        }

        private static readonly ProfilerMarker UpdateHandDataPerfMarker = new ProfilerMarker("[MRTK] OpenXRArticulatedHand.UpdateHandData");

        /// <summary>
        /// Update the hand data from the device.
        /// </summary>
        /// <param name="hand">The hand retrieved from Unity via OpenXR.</param>
        private void UpdateHandData()
        {
            using (UpdateHandDataPerfMarker.Auto())
            {
                handMeshProvider?.UpdateHandMesh();
                handJointProvider?.UpdateHandJoints(ref unityJointPoses);
                handDefinition?.UpdateHandJoints(unityJointPoses);
            }
        }
    }
}
