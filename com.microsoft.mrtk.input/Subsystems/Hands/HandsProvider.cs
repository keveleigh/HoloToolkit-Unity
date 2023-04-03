using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Provides a generic, base hands provider for subsystem use.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="HandDataContainer"/> to use with this provider.</typeparam>
    [Preserve]
    internal class HandsProvider<T> : HandsSubsystem.Provider where T : HandDataContainer
    {
        private Dictionary<XRNode, T> hands = null;

        public override void Start()
        {
            base.Start();

            hands ??= new Dictionary<XRNode, T>
            {
                { XRNode.LeftHand, Activator.CreateInstance(typeof(T), XRNode.LeftHand) as T },
                { XRNode.RightHand, Activator.CreateInstance(typeof(T), XRNode.RightHand) as T }
            };

            InputSystem.onBeforeUpdate += ResetHands;
        }

        public override void Stop()
        {
            ResetHands();
            InputSystem.onBeforeUpdate -= ResetHands;
            base.Stop();
        }

        private void ResetHands()
        {
            hands[XRNode.LeftHand].Reset();
            hands[XRNode.RightHand].Reset();
        }

        #region IHandsSubsystem implementation

        /// <inheritdoc/>
        public override bool TryGetEntireHand(XRNode handNode, out IReadOnlyList<HandJointPose> jointPoses)
        {
            Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, "Non-hand XRNode used in TryGetEntireHand query.");
            return hands[handNode].TryGetEntireHand(out jointPoses);
        }

        /// <inheritdoc/>
        public override bool TryGetJoint(TrackedHandJoint joint, XRNode handNode, out HandJointPose jointPose)
        {
            Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, "Non-hand XRNode used in TryGetJoint query.");
            return hands[handNode].TryGetJoint(joint, out jointPose);
        }

        #endregion IHandsSubsystem implementation
    }
}
