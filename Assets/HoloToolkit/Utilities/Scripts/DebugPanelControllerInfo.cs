// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using System.Collections.Generic;
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity
{
    public class DebugPanelControllerInfo : MonoBehaviour
    {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        private class ControllerState
        {
            public InteractionSourceHandedness Handedness;
            public Vector3 PointerPosition;
            public Quaternion PointerRotation;
            public Vector3 GripPosition;
            public Quaternion GripRotation;
            public bool Grasped;
            public bool MenuPressed;
            public bool SelectPressed;
            public float SelectPressedAmount;
            public bool ThumbstickPressed;
            public Vector2 ThumbstickPosition;
            public bool TouchpadPressed;
            public bool TouchpadTouched;
            public Vector2 TouchpadPosition;
        }

        private Dictionary<uint, ControllerState> controllers;
#endif

        // Text display label game objects
        public TextMesh LeftInfoTextPointerPosition;
        public TextMesh LeftInfoTextPointerRotation;
        public TextMesh LeftInfoTextGripPosition;
        public TextMesh LeftInfoTextGripRotation;
        public TextMesh LeftInfoTextGripGrasped;
        public TextMesh LeftInfoTextMenuPressed;
        public TextMesh LeftInfoTextTriggerPressed;
        public TextMesh LeftInfoTextTriggerPressedAmount;
        public TextMesh LeftInfoTextThumbstickPressed;
        public TextMesh LeftInfoTextThumbstickPosition;
        public TextMesh LeftInfoTextTouchpadPressed;
        public TextMesh LeftInfoTextTouchpadTouched;
        public TextMesh LeftInfoTextTouchpadPosition;
        public TextMesh RightInfoTextPointerPosition;
        public TextMesh RightInfoTextPointerRotation;
        public TextMesh RightInfoTextGripPosition;
        public TextMesh RightInfoTextGripRotation;
        public TextMesh RightInfoTextGripGrasped;
        public TextMesh RightInfoTextMenuPressed;
        public TextMesh RightInfoTextTriggerPressed;
        public TextMesh RightInfoTextTriggerPressedAmount;
        public TextMesh RightInfoTextThumbstickPressed;
        public TextMesh RightInfoTextThumbstickPosition;
        public TextMesh RightInfoTextTouchpadPressed;
        public TextMesh RightInfoTextTouchpadTouched;
        public TextMesh RightInfoTextTouchpadPosition;

        private void Awake()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            controllers = new Dictionary<uint, ControllerState>();

            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;

            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
#endif
        }

        private void Start()
        {
            if (DebugPanel.Instance != null)
            {
                DebugPanel.Instance.RegisterExternalLogCallback(GetControllerInfo);
            }
        }

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
        {
            Debug.LogFormat("{0} {1} Detected", obj.state.source.handedness, obj.state.source.kind);

            if (obj.state.source.kind == InteractionSourceKind.Controller && !controllers.ContainsKey(obj.state.source.id))
            {
                controllers.Add(obj.state.source.id, new ControllerState { Handedness = obj.state.source.handedness });
            }
        }

        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
        {
            Debug.LogFormat("{0} {1} Lost", obj.state.source.handedness, obj.state.source.kind);

            controllers.Remove(obj.state.source.id);
        }

        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
        {
            ControllerState controllerState;
            if (controllers.TryGetValue(obj.state.source.id, out controllerState))
            {
                obj.state.sourcePose.TryGetPosition(out controllerState.PointerPosition, InteractionSourceNode.Pointer);
                obj.state.sourcePose.TryGetRotation(out controllerState.PointerRotation, InteractionSourceNode.Pointer);
                obj.state.sourcePose.TryGetPosition(out controllerState.GripPosition, InteractionSourceNode.Grip);
                obj.state.sourcePose.TryGetRotation(out controllerState.GripRotation, InteractionSourceNode.Grip);

                controllerState.Grasped = obj.state.grasped;
                controllerState.MenuPressed = obj.state.menuPressed;
                controllerState.SelectPressed = obj.state.selectPressed;
                controllerState.SelectPressedAmount = obj.state.selectPressedAmount;
                controllerState.ThumbstickPressed = obj.state.thumbstickPressed;
                controllerState.ThumbstickPosition = obj.state.thumbstickPosition;
                controllerState.TouchpadPressed = obj.state.touchpadPressed;
                controllerState.TouchpadTouched = obj.state.touchpadTouched;
                controllerState.TouchpadPosition = obj.state.touchpadPosition;
            }
        }
#endif

        private string GetControllerInfo()
        {
            string toReturn = string.Empty;
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            foreach (ControllerState controllerState in controllers.Values)
            {
                string controllerStateHandedness = controllerState.Handedness.ToString();
                string controllerStatePointerPosition = controllerState.PointerPosition.ToString();
                string controllerStatePointerRotation = controllerState.PointerRotation.eulerAngles.ToString();
                string controllerStateGripPosition = controllerState.GripPosition.ToString();
                string controllerStateGripRotation = controllerState.GripRotation.eulerAngles.ToString();
                string controllerStateGrasped = controllerState.Grasped.ToString();
                string controllerStateMenuPressed = controllerState.MenuPressed.ToString();
                string controllerStateSelectPressed = controllerState.SelectPressed.ToString();
                string controllerStateSelectPressedAmount = controllerState.SelectPressedAmount.ToString();
                string controllerStateThumbstickPressed = controllerState.ThumbstickPressed.ToString();
                string controllerStateThumbstickPosition = controllerState.ThumbstickPosition.ToString();
                string controllerStateTouchpadPressed = controllerState.TouchpadPressed.ToString();
                string controllerStateTouchpadTouched = controllerState.TouchpadTouched.ToString();
                string controllerStateTouchpadPosition = controllerState.TouchpadPosition.ToString();

                // Debug message
                toReturn += string.Format("Hand: {0}\nPointer: Position: {1} Rotation: {2}\n" +
                                          "Grip: Position: {3} Rotation: {4}\nGrasped: {5} " +
                                          "MenuPressed: {6}\nSelect: Pressed: {7} PressedAmount: {8}\n" +
                                          "Thumbstick: Pressed: {9} Position: {10}\nTouchpad: Pressed: {11} " +
                                          "Touched: {12} Position: {13}\n\n",
                                          controllerStateHandedness, controllerStatePointerPosition, controllerStatePointerRotation,
                                          controllerStateGripPosition, controllerStateGripRotation, controllerStateGrasped,
                                          controllerStateMenuPressed, controllerStateSelectPressed, controllerStateSelectPressedAmount,
                                          controllerStateThumbstickPressed, controllerStateThumbstickPosition, controllerStateTouchpadPressed,
                                          controllerStateTouchpadTouched, controllerStateTouchpadPosition);

                // Text label display
                if (controllerState.Handedness.Equals(InteractionSourceHandedness.Left))
                {
                    LeftInfoTextPointerPosition.text = controllerStateHandedness;
                    LeftInfoTextPointerRotation.text = controllerStatePointerRotation;
                    LeftInfoTextGripPosition.text = controllerStateGripPosition;
                    LeftInfoTextGripRotation.text = controllerStateGripRotation;
                    LeftInfoTextGripGrasped.text = controllerStateGrasped;
                    LeftInfoTextMenuPressed.text = controllerStateMenuPressed;
                    LeftInfoTextTriggerPressed.text = controllerStateSelectPressed;
                    LeftInfoTextTriggerPressedAmount.text = controllerStateSelectPressedAmount;
                    LeftInfoTextThumbstickPressed.text = controllerStateThumbstickPressed;
                    LeftInfoTextThumbstickPosition.text = controllerStateThumbstickPosition;
                    LeftInfoTextTouchpadPressed.text = controllerStateTouchpadPressed;
                    LeftInfoTextTouchpadTouched.text = controllerStateTouchpadTouched;
                    LeftInfoTextTouchpadPosition.text = controllerStateTouchpadPosition;
                }
                else if (controllerState.Handedness.Equals(InteractionSourceHandedness.Right))
                {
                    RightInfoTextPointerPosition.text = controllerStatePointerPosition;
                    RightInfoTextPointerRotation.text = controllerStatePointerRotation;
                    RightInfoTextGripPosition.text = controllerStateGripPosition;
                    RightInfoTextGripRotation.text = controllerStateGripRotation;
                    RightInfoTextGripGrasped.text = controllerStateGrasped;
                    RightInfoTextMenuPressed.text = controllerStateMenuPressed;
                    RightInfoTextTriggerPressed.text = controllerStateSelectPressed;
                    RightInfoTextTriggerPressedAmount.text = controllerStateSelectPressedAmount;
                    RightInfoTextThumbstickPressed.text = controllerStateThumbstickPressed;
                    RightInfoTextThumbstickPosition.text = controllerStateThumbstickPosition;
                    RightInfoTextTouchpadPressed.text = controllerStateTouchpadPressed;
                    RightInfoTextTouchpadTouched.text = controllerStateTouchpadTouched;
                    RightInfoTextTouchpadPosition.text = controllerStateTouchpadPosition;
                }
            }
#endif
            return toReturn.Substring(0, Math.Max(0, toReturn.Length - 2));
        }
    }
}