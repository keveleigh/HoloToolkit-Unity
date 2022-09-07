// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A pose source which represents a hand ray. This hand ray is constructed by deriving it from the
    /// palm and knuckle positions
    /// </summary>
    [Serializable]
    public class PolyfillHandRayPoseSource : HandBasedPoseSource
    {
        // The Hand Ray used to calculate the polyfill.
        private HandRay handRay = new HandRay();

        /// <summary>
        /// Tries to get the pose of the hand ray in world space by deriving it from the
        /// palm and knuckle positions
        /// </summary>
        public override bool TryGetPose(out Pose pose)
        {
            XRNode? handNode = Hand.ToXRNode();
            // Tick the hand ray generator function. Uses index knuckle for position.
            if (handNode.HasValue
                && HandsAggregator != null
                && HandsAggregator.TryGetJoint(TrackedHandJoint.IndexProximal, handNode.Value, out HandJointPose knuckle)
                && HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, handNode.Value, out HandJointPose palm))
            {
                handRay.Update(knuckle.Position, -palm.Up, Camera.main.transform, Hand);
                Ray ray = handRay.Ray;

                // controllerState is in rig-local space, our ray generator works in worldspace!
                pose.position = PlayspaceUtilities.ReferenceTransform.InverseTransformPoint(ray.origin);
                pose.rotation = Quaternion.LookRotation(PlayspaceUtilities.ReferenceTransform.InverseTransformVector(ray.direction),
                                                        PlayspaceUtilities.ReferenceTransform.InverseTransformVector(palm.Up));
                return true;
            }

            pose = Pose.identity;
            return false;
        }
    }
}
