// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class TestHaptics : InputSystemGlobalHandlerListener, IMixedRealityInputHandler
    {
        [SerializeField]
        private MixedRealityInputAction hapticsTriggerAction;

        protected override void RegisterHandlers()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler>(this);
        }

        protected override void UnregisterHandlers()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler>(this);
        }

        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == hapticsTriggerAction
                && eventData.InputSource.Pointers[0].Controller is IMixedRealityHapticFeedback hapticFeedback)
            {
                hapticFeedback.StartHapticImpulse(1);
            }
        }

        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == hapticsTriggerAction
                && eventData.InputSource.Pointers[0].Controller is IMixedRealityHapticFeedback hapticFeedback)
            {
                hapticFeedback.StopHapticFeedback();
            }
        }
    }
}
