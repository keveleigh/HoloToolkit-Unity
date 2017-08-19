// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Examples.ColorPicker
{
    public class GazeableColorPicker : MonoBehaviour, IFocusable, IInputClickHandler
    {
        [Serializable]
        public class ColorCallback : UnityEvent<Color> { }

        public ColorCallback OnGazedColor = new ColorCallback();
        public ColorCallback OnPickedColor = new ColorCallback();

        private IPointingSource pointer;
        private Renderer rendererComponent;

        private void Awake()
        {
            rendererComponent = gameObject.EnsureComponent<Renderer>();
        }

        private void Update()
        {
            if (pointer == null)
            {
                return;
            }

            UpdatePickedColor(OnGazedColor);
        }

        private void UpdatePickedColor(ColorCallback colorCallback)
        {
            if (pointer != null)
            {
                FocusDetails focusDetails = FocusManager.Instance.GetFocusDetails(pointer);

                if (focusDetails.Object != rendererComponent.gameObject)
                {
                    return;
                }

                var texture = (Texture2D)rendererComponent.material.mainTexture;

                Vector2 pixelTextureCoord = focusDetails.Hit.textureCoord;
                pixelTextureCoord.x *= texture.width;
                pixelTextureCoord.y *= texture.height;

                Color col = texture.GetPixel((int)pixelTextureCoord.x, (int)pixelTextureCoord.y);
                colorCallback.Invoke(col);
            }
        }

        public void OnFocusEnter()
        {
            pointer = FocusManager.Instance.TryGetSinglePointer();
        }

        public void OnFocusExit()
        {
            pointer = null;
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            UpdatePickedColor(OnPickedColor);
        }
    }
}