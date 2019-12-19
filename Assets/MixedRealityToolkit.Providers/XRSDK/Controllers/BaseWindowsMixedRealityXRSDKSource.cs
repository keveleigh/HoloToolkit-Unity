﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR;

#if WMR_ENABLED
using Unity.XR.WindowsMR;
#endif // WMR_ENABLED

namespace Microsoft.MixedReality.Toolkit.XRSDK.Input
{
    /// <summary>
    /// A Windows Mixed Reality Source instance.
    /// </summary>
    public abstract class BaseWindowsMixedRealityXRSDKSource : GenericXRSDKController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected BaseWindowsMixedRealityXRSDKSource(TrackingState trackingState, Handedness sourceHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, sourceHandedness, inputSource, interactions)
        {
        }

        private Vector3 currentPointerPosition = Vector3.zero;
        private Quaternion currentPointerRotation = Quaternion.identity;
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// Update spatial pointer and spatial grip data.
        /// </summary>
        protected override void UpdatePoseData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);

            base.UpdatePoseData(interactionMapping, inputDevice);

            // Update the interaction data source
            switch (interactionMapping.InputType)
            {
#if WMR_ENABLED
                case DeviceInputType.SpatialPointer:
                    if (inputDevice.TryGetFeatureValue(WindowsMRUsages.PointerPosition, out currentPointerPosition))
                    {
                        currentPointerPose.Position = MixedRealityPlayspace.TransformPoint(currentPointerPosition);
                    }

                    if (inputDevice.TryGetFeatureValue(WindowsMRUsages.PointerRotation, out currentPointerRotation))
                    {
                        currentPointerPose.Rotation = MixedRealityPlayspace.Rotation * currentPointerRotation;
                    }

                    interactionMapping.PoseData = currentPointerPose;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system event if it's enabled
                        CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
                    }
                    break;
#endif // WMR_ENABLED
                default:
                    return;
            }
        }
    }
}
