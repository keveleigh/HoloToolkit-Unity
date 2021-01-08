﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.UnityInput
{
    /// <summary>
    /// Xbox Controller using Unity Input System
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.Xbox,
        new[] { Handedness.None },
        "Textures/XboxController")]
    public class XboxController : GenericJoystickController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public XboxController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions, new XboxControllerDefinition())
        {
        }

        /// <inheritdoc />
        [Obsolete("The DefaultInteractions property is obsolete and will be removed in a future version of the Mixed Reality Toolkit. Please use ControllerDefinition to define interactions.")]
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Left Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInteractionMapping(1, "Left Thumbstick Click", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(2, "Right Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInteractionMapping(3, "Right Thumbstick Click", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(4, "D-Pad", AxisType.DualAxis, DeviceInputType.DirectionalPad),
            new MixedRealityInteractionMapping(5, "Shared Trigger", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(6, "Left Trigger", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(7, "Right Trigger", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(8, "View", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(9, "Menu", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(10, "Left Bumper", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(11, "Right Bumper", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(12, "A", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(13, "B", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(14, "X", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(15, "Y", AxisType.Digital, DeviceInputType.ButtonPress),
        };
    }
}