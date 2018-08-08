// Copyright(c) Microsoft Corporation.All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.SDK.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class ControllerDataDemo : InputSystemGlobalListener, IMixedRealitySourceStateHandler, IMixedRealitySpatialInputHandler
    {
        public TextMesh LeftPointerPositionText;
        public TextMesh LeftPointerRotationText;
        public TextMesh LeftGripPositionText;
        public TextMesh LeftGripRotationText;
        public TextMesh LeftGripGraspedText;
        public TextMesh LeftMenuPressedText;
        public TextMesh LeftTriggerPressedText;
        public TextMesh LeftTriggerPressedAmountText;
        public TextMesh LeftThumbstickPressedText;
        public TextMesh LeftThumbstickPositionText;
        public TextMesh LeftTouchpadPressedText;
        public TextMesh LeftTouchpadTouchedText;
        public TextMesh LeftTouchpadPositionText;
        public TextMesh RightPointerPositionText;
        public TextMesh RightPointerRotationText;
        public TextMesh RightGripPositionText;
        public TextMesh RightGripRotationText;
        public TextMesh RightGripGraspedText;
        public TextMesh RightMenuPressedText;
        public TextMesh RightTriggerPressedText;
        public TextMesh RightTriggerPressedAmountText;
        public TextMesh RightThumbstickPressedText;
        public TextMesh RightThumbstickPositionText;
        public TextMesh RightTouchpadPressedText;
        public TextMesh RightTouchpadTouchedText;
        public TextMesh RightTouchpadPositionText;

        public MixedRealityInputAction PointerPoseAction;
        public MixedRealityInputAction GripPoseAction;
        public MixedRealityInputAction GripGraspedAction;
        public MixedRealityInputAction MenuPressedAction;
        public MixedRealityInputAction TriggerPressedAction;
        public MixedRealityInputAction TriggerPressedAmountAction;
        public MixedRealityInputAction ThumbstickPressedAction;
        public MixedRealityInputAction ThumbstickPositionAction;
        public MixedRealityInputAction TouchpadPressedAction;
        public MixedRealityInputAction TouchpadTouchedAction;
        public MixedRealityInputAction TouchpadPositionAction;

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
        {
            Debug.LogFormat("{0} Source Detected", eventData.Controller.ControllerHandedness);
        }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            Debug.LogFormat("{0} Source Lost", eventData.Controller.ControllerHandedness);
        }

        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == MenuPressedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftMenuPressedText : RightMenuPressedText).text = "True";
            }
            else if (eventData.MixedRealityInputAction == TriggerPressedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftTriggerPressedText : RightTriggerPressedText).text = "True";
            }
            else if (eventData.MixedRealityInputAction == TouchpadTouchedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftTouchpadTouchedText : RightTouchpadTouchedText).text = "True";
            }
            else if (eventData.MixedRealityInputAction == GripGraspedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftGripGraspedText : RightGripGraspedText).text = "True";
            }
            else if (eventData.MixedRealityInputAction == TouchpadPressedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftTouchpadPressedText : RightTouchpadPressedText).text = "True";
            }
            else if (eventData.MixedRealityInputAction == ThumbstickPressedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftThumbstickPressedText : RightThumbstickPressedText).text = "True";
            }
        }

        void IMixedRealityInputHandler.OnInputPressed(InputEventData<float> eventData)
        {
            if (eventData.MixedRealityInputAction == TriggerPressedAmountAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftTriggerPressedAmountText : RightTriggerPressedAmountText).text = eventData.InputData.ToString();
            }
        }

        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == MenuPressedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftMenuPressedText : RightMenuPressedText).text = "False";
            }
            else if (eventData.MixedRealityInputAction == TriggerPressedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftTriggerPressedText : RightTriggerPressedText).text = "False";
            }
            else if (eventData.MixedRealityInputAction == TouchpadTouchedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftTouchpadTouchedText : RightTouchpadTouchedText).text = "False";
            }
            else if (eventData.MixedRealityInputAction == GripGraspedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftGripGraspedText : RightGripGraspedText).text = "False";
            }
            else if (eventData.MixedRealityInputAction == TouchpadPressedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftTouchpadPressedText : RightTouchpadPressedText).text = "False";
            }
            else if (eventData.MixedRealityInputAction == ThumbstickPressedAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftThumbstickPressedText : RightThumbstickPressedText).text = "False";
            }
        }

        void IMixedRealitySpatialInputHandler.OnPoseInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.MixedRealityInputAction == GripPoseAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftGripPositionText : RightGripPositionText).text = eventData.InputData.Position.ToString();
                (eventData.Handedness == Handedness.Left ? LeftGripRotationText : RightGripRotationText).text = eventData.InputData.Rotation.eulerAngles.ToString();
            }
            else if (eventData.MixedRealityInputAction == PointerPoseAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftPointerPositionText : RightPointerPositionText).text = eventData.InputData.Position.ToString();
                (eventData.Handedness == Handedness.Left ? LeftPointerRotationText : RightPointerRotationText).text = eventData.InputData.Rotation.eulerAngles.ToString();
            }
        }

        void IMixedRealitySpatialInputHandler.OnPositionChanged(InputEventData<Vector3> eventData)
        {
            // Nothing
        }

        void IMixedRealityInputHandler.OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
            if (eventData.MixedRealityInputAction == ThumbstickPositionAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftThumbstickPositionText : RightThumbstickPositionText).text = eventData.InputData.ToString();
            }
            else if (eventData.MixedRealityInputAction == TouchpadPositionAction)
            {
                (eventData.Handedness == Handedness.Left ? LeftTouchpadPositionText : RightTouchpadPositionText).text = eventData.InputData.ToString();
            }
        }

        void IMixedRealitySpatialInputHandler.OnRotationChanged(InputEventData<Quaternion> eventData)
        {
            // Nothing
        }
    }
}