// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines the interactions and data that a Windows Mixed Reality motion controller can provide.
    /// </summary>
    public class WindowsMixedRealityControllerDefinition
    {
        public WindowsMixedRealityControllerDefinition(IMixedRealityInputSource source, Handedness handedness)
        {
            inputSource = source;
            this.handedness = handedness;
        }

        protected readonly IMixedRealityInputSource inputSource;
        protected readonly Handedness handedness;

        /// <summary>
        /// A Windows Mixed Reality motion controller's default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public MixedRealityInputActionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInputActionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInputActionMapping("Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
            new MixedRealityInputActionMapping("Grip Press", AxisType.SingleAxis, DeviceInputType.GripPress),
            new MixedRealityInputActionMapping("Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInputActionMapping("Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInputActionMapping("Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInputActionMapping("Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad),
            new MixedRealityInputActionMapping("Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch),
            new MixedRealityInputActionMapping("Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress),
            new MixedRealityInputActionMapping("Menu Press", AxisType.Digital, DeviceInputType.Menu),
            new MixedRealityInputActionMapping("Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInputActionMapping("Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress)
        };
    }
}
