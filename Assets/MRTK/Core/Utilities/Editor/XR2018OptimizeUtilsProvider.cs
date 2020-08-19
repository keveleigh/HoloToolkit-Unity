// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;

public class XR2018OptimizeUtilsProvider : IMixedRealityOptimizeUtilsProvider
{
    bool IMixedRealityOptimizeUtilsProvider.IsValid =>
#if UNITY_2020_1_OR_NEWER
        false;
#else
        XRSettingsUtilities.IsLegacyXRActive;
#endif

    bool IMixedRealityOptimizeUtilsProvider.IsOptimalRenderingPath
    {
        get
#if UNITY_ANDROID
            => PlayerSettings.stereoRenderingPath == StereoRenderingPath.SinglePass;
#else
            => PlayerSettings.stereoRenderingPath == StereoRenderingPath.Instancing;
#endif
        set
#if UNITY_ANDROID
            => PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
#else
            => PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;
#endif
    }

    bool IMixedRealityOptimizeUtilsProvider.DepthBufferSharingEnabled
    {
        get
        {
#if !UNITY_2020_1_OR_NEWER
            if (MixedRealityOptimizeUtils.IsBuildTargetOpenVR())
            {
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                if (PlayerSettings.VROculus.sharedDepthBuffer)
#pragma warning restore 0618
                {
                    return true;
                }
            }
            else if (MixedRealityOptimizeUtils.IsBuildTargetUWP())
            {
#if UNITY_2019_1_OR_NEWER
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                if (PlayerSettings.VRWindowsMixedReality.depthBufferSharingEnabled)
#pragma warning restore 0618
                {
                    return true;
                }
#else
                var playerSettings = GetSettingsObject("PlayerSettings");
                var property = playerSettings?.FindProperty("vrSettings.hololens.depthBufferSharingEnabled");
                if (property != null && property.boolValue)
                {
                    return true;
                }
#endif // UNITY_2019_1_OR_NEWER
            }
#endif // !UNITY_2020_1_OR_NEWER

            return false;
        }
        set
        {
#if !UNITY_2020_1_OR_NEWER
            if (MixedRealityOptimizeUtils.IsBuildTargetOpenVR())
            {
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                PlayerSettings.VROculus.sharedDepthBuffer = value;
#pragma warning restore 0618
            }
            else if (MixedRealityOptimizeUtils.IsBuildTargetUWP())
            {
#if UNITY_2019_1_OR_NEWER
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                PlayerSettings.VRWindowsMixedReality.depthBufferSharingEnabled = value;
#pragma warning restore 0618
#else
                var playerSettings = MixedRealityOptimizeUtils.GetSettingsObject("PlayerSettings");
                MixedRealityOptimizeUtils.ChangeProperty(playerSettings,
                    "vrSettings.hololens.depthBufferSharingEnabled",
                    property => property.boolValue = enableDepthBuffer);
#endif // UNITY_2019_1_OR_NEWER
            }
#endif // !UNITY_2020_1_OR_NEWER
        }
    }

    bool IMixedRealityOptimizeUtilsProvider.IsDepthBufferFormat16Bit
    {
        get
        {
#if UNITY_2019_1_OR_NEWER
#pragma warning disable 0618
            return PlayerSettings.VRWindowsMixedReality.depthBufferFormat == PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat16Bit;
#pragma warning restore 0618
#else
            var playerSettings = MixedRealityOptimizeUtils.GetSettingsObject("PlayerSettings");
            var property = playerSettings?.FindProperty("vrSettings.hololens.depthFormat");
            return property != null && property.intValue == 0;
#endif
        }
        set
        {
            int depthFormat = value ? 0 : 1;

#if !UNITY_2020_1_OR_NEWER
            // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
            // with legacy requirements.
#if UNITY_2019_1_OR_NEWER
            // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
            // with legacy requirements.
#pragma warning disable 0618
            PlayerSettings.VRWindowsMixedReality.depthBufferFormat = value ?
                PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat16Bit :
                PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat24Bit;
#pragma warning restore 0618
#else
            var playerSettings = MixedRealityOptimizeUtils.GetSettingsObject("PlayerSettings");
            MixedRealityOptimizeUtils.ChangeProperty(playerSettings,
                "vrSettings.hololens.depthFormat",
                property => property.intValue = depthFormat);
#endif // UNITY_2019_1_OR_NEWER
#endif // !UNITY_2020_1_OR_NEWER
        }
    }
}
