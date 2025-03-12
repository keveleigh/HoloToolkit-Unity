using Microsoft.MixedReality.Toolkit.Input;
using System;

#if UNITY_OPENXR
using UnityEngine.XR.OpenXR;
#endif

namespace Microsoft.MixedReality.Toolkit.XRSDK.AndroidXR
{
    public class OpenXRDeviceManager : OpenXR.OpenXRDeviceManager
    {
        public OpenXRDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = 10,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        /// <inheritdoc />
        protected override Type GetControllerType(SupportedControllerType supportedControllerType)
        {
#if UNITY_OPENXR
            switch (supportedControllerType)
            {
                case SupportedControllerType.ArticulatedHand:
                    if (OpenXRRuntime.IsExtensionEnabled("XR_EXT_hand_interaction"))
                    {
                        return typeof(OpenXRArticulatedHand);
                    }
                    break;
            }
#endif
            return base.GetControllerType(supportedControllerType);
        }
    }
}
