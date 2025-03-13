using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;

#if UNITY_OPENXR
using UnityEngine.XR.OpenXR;
#endif

namespace Microsoft.MixedReality.Toolkit.XRSDK.AndroidXR
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1),
        "OpenXR Device Manager",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
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
            if (supportedControllerType == SupportedControllerType.ArticulatedHand && OpenXRRuntime.IsExtensionEnabled("XR_EXT_hand_interaction"))
            {
                return typeof(OpenXRArticulatedHand);
            }
#endif
            return base.GetControllerType(supportedControllerType);
        }
    }
}
