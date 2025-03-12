// Copyright (c) Qualcomm Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

#if UNITY_XR_HANDS
using System.Collections.Generic;
using UnityEngine.XR.Hands;
#endif

namespace Microsoft.MixedReality.Toolkit.XRSDK.AndroidXR
{
    internal class UnityXRHandJointProvider
    {
        public UnityXRHandJointProvider(Handedness handedness)
        {
            this.handedness = handedness;

#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(HandTrackingPermission))
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += OnPermissionDenied;
                callbacks.PermissionGranted += OnPermissionGranted;

                Permission.RequestUserPermission(HandTrackingPermission, callbacks);
                Debug.Log($"Requesting {HandTrackingPermission}!");
            }
            else
            {
                Debug.Log($"{HandTrackingPermission} already granted!");
            }
        }

        const string HandTrackingPermission = "android.permission.HAND_TRACKING";

        void OnPermissionDenied(string permission)
        {
            Debug.Log($"{HandTrackingPermission} denied :(");
        }

        void OnPermissionGranted(string permission)
        {
            Debug.Log($"{HandTrackingPermission} newly granted! :)");
#endif // UNITY_ANDROID
        }

        private readonly Handedness handedness;

#if UNITY_XR_HANDS
        private static XRHandSubsystem handSubsystem = null;
        private static readonly List<XRHandSubsystem> XRHandSubsystems = new List<XRHandSubsystem>();
#endif

        public void UpdateHandJoints(ref MixedRealityPose[] jointPoses)
        {
#if UNITY_XR_HANDS
            XRHand xrHand = GetTrackedHand();

            /// <summary>
            /// Obtains a reference to the actual XRHand object representing the tracked hand
            /// functionality present on HandNode.
            /// </summary>
            XRHand GetTrackedHand()
            {
                if (handSubsystem == null || !handSubsystem.running)
                {
                    handSubsystem = null;
                    SubsystemManager.GetSubsystems(XRHandSubsystems);
                    foreach (XRHandSubsystem xrHandSubsystem in XRHandSubsystems)
                    {
                        if (xrHandSubsystem.running)
                        {
                            handSubsystem = xrHandSubsystem;
                            break;
                        }
                    }

                    if (handSubsystem == null)
                    {
                        Debug.Log("NO SUBSYSTEM?!?!?!?");
                        // No hand subsystem running this frame.
                        return default;
                    }
                }

                return handedness.HasFlag(Handedness.Left) ? handSubsystem.leftHand : handSubsystem.rightHand;
            }

            if (xrHand.isTracked)
            {
                Debug.Log("HAND TRACKED!!!");
                for (int i = (int)XRHandJointID.BeginMarker; i < (int)XRHandJointID.EndMarker; i++)
                {
                    if (xrHand.GetJoint((XRHandJointID)i).TryGetPose(out Pose pose))
                    {
                        if (jointPoses == null)
                        {
                            jointPoses = new MixedRealityPose[ArticulatedHandPose.JointCount];
                        }

                        // We want input sources to follow the playspace, so fold in the playspace transform here to
                        // put the pose into world space.
                        Vector3 position = MixedRealityPlayspace.TransformPoint(pose.position);
                        Quaternion rotation = MixedRealityPlayspace.Rotation * pose.rotation;

                        jointPoses[i] = new MixedRealityPose(position, rotation);
                    }
                }
            }
            else
            {
                Debug.Log("HAND NOT TRACKEDDDDDD :(");
            }
#endif
        }
    }
}
