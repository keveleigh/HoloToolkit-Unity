// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Camera settings provider for use with Windows Mixed Reality.
    /// </summary>
    public class XRSDKWindowsMixedRealityCameraSettings : BaseWindowsMixedRealityCameraSettings
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cameraSystem">The instance of the camera system which is managing this provider.</param>
        /// <param name="name">Friendly name of the provider.</param>
        /// <param name="priority">Provider priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The provider's configuration profile.</param>
        public XRSDKWindowsMixedRealityCameraSettings(
            IMixedRealityCameraSystem cameraSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseCameraSettingsProfile profile = null) : base(cameraSystem, name, priority, profile)
        { }

        private XRDisplaySubsystem displaySubsystem;
        private XRDisplaySubsystem DisplaySubsystem
        {
            get
            {
                if (displaySubsystem == null &&
                    XRGeneralSettings.Instance != null &&
                    XRGeneralSettings.Instance.Manager != null &&
                    XRGeneralSettings.Instance.Manager.activeLoader != null)
                {
                    displaySubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRDisplaySubsystem>();
                }

                return displaySubsystem;
            }
        }

        #region IMixedRealityCameraSettings

        /// <inheritdoc/>
        public override bool IsOpaque =>
#if UNITY_WSA
            DisplaySubsystem?.displayOpaque ?? true;
#else
            false;
#endif

        #endregion IMixedRealityCameraSettings
    }
}
