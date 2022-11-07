// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An ArticulatedTouchInteractor that is driven by the
    /// pinching point on the specified hand, as defined by the
    /// HandsAggregatorSubsystem. This is typically the averaged pose
    /// between the index tip and the thumb tip.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Grab Interactor")]
    public class GrabInteractor : HandJointInteractor, IGrabInteractor
    {
        [SerializeReference]
        [InterfaceSelector(true)]
        [Tooltip("The pose source representing the worldspace pose of the hand pinching point.")]
        private IPoseSource pinchPoseSource;

        /// <summary>
        /// The pose source representing the worldspace pose of the hand pinching point.
        /// </summary>
        protected IPoseSource PinchPoseSource { get => pinchPoseSource; set => pinchPoseSource = value; }

        [SerializeReference]
        [InterfaceSelector(true)]
        [Tooltip("The pose source representing the worldspace pose of the hand grabbing point.")]
        private IPoseSource gripPoseSource;

        /// <summary>
        /// The pose source representing the worldspace pose of the hand pinching point.
        /// </summary>
        protected IPoseSource GripPoseSource { get => gripPoseSource; set => gripPoseSource = value; }

        /// <summary>
        /// Get near interaction point from hands aggregator.
        /// </summary>
        protected override bool TryGetInteractionPoint(out Pose pose)
        {
            pose = Pose.identity;
            return PinchPoseSource != null && PinchPoseSource.TryGetPose(out pose);
        }

        private static readonly ProfilerMarker ProcessInteractorPerfMarker =
            new ProfilerMarker("[MRTK] GrabInteractor.ProcessInteractor");

        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            using (ProcessInteractorPerfMarker.Auto())
            {
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic
                    && hasSelection
                    && firstInteractableSelected is MRTKGrabInteractable grabInteractable
                    && grabInteractable.AlignWithSqueeze
                    && GripPoseSource != null
                    && GripPoseSource.TryGetPose(out Pose pose))
                {
                    // Ensure that the attachTransform tightly follows the grip pose
                    attachTransform.SetPositionAndRotation(pose.position, pose.rotation);
                }
            }
        }
    }
}
