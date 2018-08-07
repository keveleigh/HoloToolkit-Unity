// Copyright(c) Microsoft Corporation.All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.SDK.Input;
using System.Collections.Generic;
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
            switch (eventData.MixedRealityInputAction.Description)
            {
                case "Menu":
                    (eventData.Handedness == Handedness.Left ? LeftMenuPressedText : RightMenuPressedText).text = "True";
                    break;
                case "Select":
                    (eventData.Handedness == Handedness.Left ? LeftTriggerPressedText : RightTriggerPressedText).text = "True";
                    break;
                case "Touchpad Touch":
                    (eventData.Handedness == Handedness.Left ? LeftTouchpadTouchedText : RightTouchpadTouchedText).text = "True";
                    break;
                case "Grip Press":
                    (eventData.Handedness == Handedness.Left ? LeftGripGraspedText : RightGripGraspedText).text = "True";
                    break;
                case "Touchpad Press":
                    (eventData.Handedness == Handedness.Left ? LeftTouchpadPressedText : RightTouchpadPressedText).text = "True";
                    break;
                case "Thumbstick Press":
                    (eventData.Handedness == Handedness.Left ? LeftThumbstickPressedText : RightThumbstickPressedText).text = "True";
                    break;
            }
        }

        void IMixedRealityInputHandler.OnInputPressed(InputEventData<float> eventData)
        {
            switch (eventData.MixedRealityInputAction.Description)
            {
                case "Trigger Position":
                    (eventData.Handedness == Handedness.Left ? LeftTriggerPressedAmountText : RightTriggerPressedAmountText).text = eventData.InputData.ToString();
                    break;
            }
        }

        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData)
        {
            switch (eventData.MixedRealityInputAction.Description)
            {
                case "Menu":
                    (eventData.Handedness == Handedness.Left ? LeftMenuPressedText : RightMenuPressedText).text = "False";
                    break;
                case "Select":
                    (eventData.Handedness == Handedness.Left ? LeftTriggerPressedText : RightTriggerPressedText).text = "False";
                    break;
                case "Touchpad Touch":
                    (eventData.Handedness == Handedness.Left ? LeftTouchpadTouchedText : RightTouchpadTouchedText).text = "False";
                    break;
                case "Grip Press":
                    (eventData.Handedness == Handedness.Left ? LeftGripGraspedText : RightGripGraspedText).text = "False";
                    break;
                case "Touchpad Press":
                    (eventData.Handedness == Handedness.Left ? LeftTouchpadPressedText : RightTouchpadPressedText).text = "False";
                    break;
                case "Thumbstick Press":
                    (eventData.Handedness == Handedness.Left ? LeftThumbstickPressedText : RightThumbstickPressedText).text = "False";
                    break;
            }
        }

        void IMixedRealitySpatialInputHandler.OnPoseInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            switch (eventData.MixedRealityInputAction.Description)
            {
                case "Grip Pose":
                    (eventData.Handedness == Handedness.Left ? LeftGripPositionText : RightGripPositionText).text = eventData.InputData.Position.ToString();
                    (eventData.Handedness == Handedness.Left ? LeftGripRotationText : RightGripRotationText).text = eventData.InputData.Rotation.ToString();
                    break;
                case "Pointer Pose":
                    (eventData.Handedness == Handedness.Left ? LeftPointerPositionText : RightPointerPositionText).text = eventData.InputData.Position.ToString();
                    (eventData.Handedness == Handedness.Left ? LeftPointerRotationText : RightPointerRotationText).text = eventData.InputData.Rotation.ToString();
                    break;
            }
        }

        void IMixedRealitySpatialInputHandler.OnPositionChanged(InputEventData<Vector3> eventData)
        {
            // Nothing
        }

        void IMixedRealityInputHandler.OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
            switch (eventData.MixedRealityInputAction.Description)
            {
                case "Teleport":
                    (eventData.Handedness == Handedness.Left ? LeftThumbstickPositionText : RightThumbstickPositionText).text = eventData.InputData.ToString();
                    break;
                case "Touchpad Position":
                    (eventData.Handedness == Handedness.Left ? LeftTouchpadPositionText : RightTouchpadPositionText).text = eventData.InputData.ToString();
                    break;
            }
        }

        void IMixedRealitySpatialInputHandler.OnRotationChanged(InputEventData<Quaternion> eventData)
        {
            // Nothing
        }
    }
}