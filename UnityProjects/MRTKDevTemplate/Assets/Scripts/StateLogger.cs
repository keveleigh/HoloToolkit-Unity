#if UNITY_ANDROID
using Qualcomm.Kurtis.Spaces;
using Qualcomm.Snapdragon.Spaces;
#endif
using Microsoft.MixedReality.Toolkit.Subsystems;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Hands;
using UnityEngine.XR.OpenXR;
using XRHandedness = UnityEngine.XR.Hands.Handedness;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    public class StateLogger : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text = null;

        [SerializeField]
        private XROrigin xrOrigin = null;

        [SerializeField]
        private InputActionProperty positionAction;

        [SerializeField]
        private InputActionProperty rotationAction;

        private const int CountToNextUpdate = 20;
        private int countSinceLastUpdate = 0;
        private XRHandSubsystem handSubsystem;
        private HandsAggregatorSubsystem handsAggregatorSubsystem;
        private XRSessionSubsystem sessionSubsystem;
#if UNITY_ANDROID
        private HandMeshFeature handMeshFeature = null;
        private SystemNameFeature systemNameFeature = null;
#endif

        protected void Start()
        {
            if (text == null)
            {
                Debug.LogError("StateLogger is missing a text field in its serialization. Please set one.");
                enabled = false;
            }

            positionAction.EnableDirectAction();
            rotationAction.EnableDirectAction();

#if UNITY_ANDROID
            systemNameFeature = OpenXRSettings.Instance.GetFeature<SystemNameFeature>();
#endif
        }

        protected void Update()
        {
            if (countSinceLastUpdate-- <= 0)
            {
                countSinceLastUpdate = CountToNextUpdate;

                string runtimeName = string.IsNullOrEmpty(OpenXRRuntime.name)
                    ? "OpenXR Runtime is not available."
                    : $"{OpenXRRuntime.name} {OpenXRRuntime.version}";

                Vector3 position = positionAction.action.ReadValue<Vector3>();

                text.SetText($"{Application.productName}" +
                    $"\nUnity Version: {Application.unityVersion}" +
                    $"\nUnity OpenXR Plugin Version: {OpenXRRuntime.pluginVersion}" +
#if UNITY_ANDROID
                    $"\nSnapdragon Spaces OpenXR Plugin {typeof(BaseRuntimeFeature).Assembly.GetName().Version}" +
                    $"\nDevice name \"{(systemNameFeature != null ? systemNameFeature.SystemName : string.Empty)}\"" +
#elif UNITY_STANDALONE_WIN || UNITY_WSA
                    //$"\nMixed Reality OpenXR Plugin {typeof(MixedRealityFeaturePlugin).Assembly.GetName().Version}" +
#endif
                    $"\n{runtimeName}" +
                    $"\n{GetDisplayInfo()}" +
                    $"\nAR Session State: {ARSession.state} | {GetTrackingMode()}" +
                    $"\nXR Origin: {xrOrigin.RequestedTrackingOriginMode} (requested) | {GetOriginMode()} (actual)" +
                    "\n" +
                    $"\nPhone controller position: ({position.x}, {position.y}, {position.z})" +
                    $"\nPhone controller rotation: {rotationAction.action.ReadValue<Quaternion>()}" +
#if UNITY_ANDROID
                    //                    $"\nLeft Hand gesture state: {GetGestureState(XRHandedness.Left)}" +
                    //                    $"\nRight Hand gesture state: {GetGestureState(XRHandedness.Right)}" +
#endif
                    "\n" +
                    $"\nLeft Hand tracking state: {GetTrackingState(XRHandedness.Left)}" +
                    $"\nRight Hand tracking state: {GetTrackingState(XRHandedness.Right)}");
                //                    $"\nLeft Hand pinch state: {GetPinchState(XRHandedness.Left)}" +
                //                    $"\nRight Hand pinch state: {GetPinchState(XRHandedness.Right)}");
            }
        }

        private string GetTrackingMode()
        {
            if (sessionSubsystem == null || !sessionSubsystem.running)
            {
                sessionSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<XRSessionSubsystem>();

                if (sessionSubsystem == null)
                {
                    // No hand subsystem running this frame.
                    return "XRSessionSubsystem not found";
                }
            }

            return sessionSubsystem.currentTrackingMode.ToString();
        }

        private static string GetDisplayInfo()
        {
            XRDisplaySubsystem displaySubsystem = XRSubsystemHelpers.DisplaySubsystem;

            if (displaySubsystem == null)
            {
                // No display subsystem running this frame.
                return "XRDisplaySubsystem not found";
            }

            string opaque = displaySubsystem.displayOpaque ? "Opaque" : "Transparent";
            OpenXRSettings.RenderMode renderMode = OpenXRSettings.Instance.renderMode;
            OpenXRSettings.DepthSubmissionMode depthMode = OpenXRSettings.Instance.depthSubmissionMode;

            return $"{opaque} | {renderMode} | {depthMode}";
        }

        private static string GetOriginMode()
        {
            XRInputSubsystem inputSubsystem = XRSubsystemHelpers.InputSubsystem;

            if (inputSubsystem == null)
            {
                // No input subsystem running this frame.
                return "XRInputSubsystem not found";
            }

            return inputSubsystem.GetTrackingOriginMode().ToString();
        }

        private string GetTrackingState(XRHandedness handedness)
        {
            if (handSubsystem == null || !handSubsystem.running)
            {
                handSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<XRHandSubsystem>();

                if (handSubsystem == null)
                {
                    // No hand subsystem running this frame.
                    return "XRHandSubsystem not found";
                }
            }

            XRHand hand = handedness == XRHandedness.Left ? handSubsystem.leftHand : handSubsystem.rightHand;
            return hand.isTracked ? "Tracked" : "Not tracked";
        }

        private string GetPinchState(XRHandedness handedness)
        {
            if (handsAggregatorSubsystem == null || !handsAggregatorSubsystem.running)
            {
                handsAggregatorSubsystem = XRSubsystemHelpers.HandsAggregator;

                if (handsAggregatorSubsystem == null)
                {
                    // No hands aggregator subsystem running this frame.
                    return "HandsAggregatorSubsystem not found";
                }
            }

            XRNode handNode = handedness == XRHandedness.Left ? XRNode.LeftHand : XRNode.RightHand;

            handsAggregatorSubsystem.TryGetJoint(TrackedHandJoint.ThumbTip, handNode, out HandJointPose thumbTip);
            handsAggregatorSubsystem.TryGetJoint(TrackedHandJoint.IndexTip, handNode, out HandJointPose indexTip);

            float pinchDistance = Vector3.Distance(indexTip.Position, thumbTip.Position);

            // Otherwise, we compute a fresh finger length.
            handsAggregatorSubsystem.TryGetJoint(TrackedHandJoint.IndexProximal, handNode, out HandJointPose indexKnuckle);
            handsAggregatorSubsystem.TryGetJoint(TrackedHandJoint.IndexIntermediate, handNode, out HandJointPose indexMiddle);
            handsAggregatorSubsystem.TryGetJoint(TrackedHandJoint.IndexDistal, handNode, out HandJointPose indexDistal);

            float indexFingerLength = Vector3.Distance(indexKnuckle.Position, indexMiddle.Position) +
                                      Vector3.Distance(indexMiddle.Position, indexDistal.Position) +
                                      Vector3.Distance(indexDistal.Position, indexTip.Position);

            return handsAggregatorSubsystem.TryGetPinchProgress(handNode, out _, out bool isPinching, out float pinchAmount) ? $"{isPinching} | {pinchAmount} | {pinchDistance} | {pinchDistance / indexFingerLength}" : "Could not query";
        }

#if UNITY_ANDROID
        private string GetGestureState(XRHandedness handedness)
        {
            if (handMeshFeature == null)
            {
                handMeshFeature = OpenXRSettings.Instance.GetFeature<HandMeshFeature>();

                if (handMeshFeature == null)
                {
                    return "HandMeshFeature not found";
                }
            }

            System.Tuple<int, float, float> result = handMeshFeature.TryGetHandGestureData(handedness == XRHandedness.Left);
            return result != null ? $"{(SpacesHand.GestureType)result.Item1} | {result.Item2} | {result.Item3}" : "Gesture could not be queried";
        }
#endif
    }
}
