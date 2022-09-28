// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Microsoft.MixedReality.Toolkit.Input
{
    internal class TouchscreenController : XRBaseController
    {
        [SerializeField]
        InputActionProperty touchPointActionProperty;

        /// <summary>
        /// The Input System action to use for projecting into the scene. Must be a <see cref="UnityEngine.InputSystem.Controls.Vector2Control"/> Control.
        /// </summary>
        public InputActionProperty TouchPointActionProperty
        {
            get => touchPointActionProperty;
            set => SetInputActionProperty(ref touchPointActionProperty, value);
        }

        [SerializeField]
        InputActionProperty isTouchedActionProperty;

        /// <summary>
        /// Typically a <see cref="UnityEngine.InputSystem.Controls.ButtonControl"/> Control.
        /// </summary>
        public InputActionProperty IsTouchedActionProperty
        {
            get => isTouchedActionProperty;
            set => SetInputActionProperty(ref isTouchedActionProperty, value);
        }

        /// <inheritdoc />
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            base.UpdateTrackingInput(controllerState);

            InputAction touchPointAction = TouchPointActionProperty.action;
            InputAction isTouchedAction = IsTouchedActionProperty.action;

            if (controllerState == null || touchPointAction == null || isTouchedAction == null)
            {
                return;
            }

            Vector2 touchPoint = touchPointAction.ReadValue<Vector2>();
            bool isTouched = isTouchedAction.ReadValue<bool>();

            controllerState.inputTrackingState = isTouched ? InputTrackingState.Position | InputTrackingState.Rotation : InputTrackingState.None;

            if (isTouched)
            {
                Ray ray = Camera.main.ScreenPointToRay(touchPoint);
                controllerState.position = ray.origin;
                controllerState.rotation = Quaternion.LookRotation(ray.direction);
            }
        }

        /// <inheritdoc />
        protected override void UpdateInput(XRControllerState controllerState)
        {
            base.UpdateInput(controllerState);

            InputAction isTouchedAction = IsTouchedActionProperty.action;

            if (controllerState == null || isTouchedAction == null)
            {
                return;
            }

            bool isTouched = isTouchedAction.ReadValue<bool>();

            controllerState.ResetFrameDependentStates();
            controllerState.selectInteractionState.SetFrameState(isTouched, isTouched ? 1.0f : 0.0f);
            controllerState.activateInteractionState.SetFrameState(isTouched, isTouched ? 1.0f : 0.0f);
            controllerState.uiPressInteractionState.SetFrameState(isTouched, isTouched ? 1.0f : 0.0f);
        }

        private void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
        {
            if (Application.isPlaying)
                property.DisableDirectAction();

            property = value;

            if (Application.isPlaying && isActiveAndEnabled)
                property.EnableDirectAction();
        }
    }
}
