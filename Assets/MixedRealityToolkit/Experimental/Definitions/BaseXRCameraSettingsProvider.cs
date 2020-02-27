// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Experimental.UnityAR;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental
{
    /// <summary>
    /// Base camera settings provider for Unity platforms that require a TrackedPoseDriver for the camera.
    /// </summary>
    public abstract class BaseXRCameraSettingsProvider : BaseCameraSettingsProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cameraSystem">The instance of the camera system which is managing this provider.</param>
        /// <param name="name">Friendly name of the provider.</param>
        /// <param name="priority">Provider priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The provider's configuration profile.</param>
        protected BaseXRCameraSettingsProvider(
            IMixedRealityCameraSystem cameraSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseCameraSettingsProfile profile = null) : base(cameraSystem, name, priority, profile)
        {
            ReadProfile();
        }

        protected ArTrackedPose poseSource = ArTrackedPose.ColorCamera;
        protected ArTrackingType trackingType = ArTrackingType.RotationAndPosition;
        protected ArUpdateType updateType = ArUpdateType.UpdateAndBeforeRender;

        private void ReadProfile()
        {
            if (SettingsProfile == null)
            {
                Debug.LogWarning("A profile was not specified for the Unity AR Camera Settings provider.\nUsing Microsoft Mixed Reality Toolkit default options.");
                return;
            }

            poseSource = SettingsProfile.PoseSource;
            trackingType = SettingsProfile.TrackingType;
            updateType = SettingsProfile.UpdateType;
        }

        #region IMixedRealityCameraSettings

        #endregion IMixedRealityCameraSettings

        /// <summary>
        /// The profile used to configure the camera.
        /// </summary>
        public BaseXRCameraSettingsProfile SettingsProfile => ConfigurationProfile as BaseXRCameraSettingsProfile;
    }
}
