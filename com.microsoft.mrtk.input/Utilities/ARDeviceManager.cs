// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Microsoft.MixedReality.Toolkit
{
    public class ARDeviceManager : MonoBehaviour
    {
        [SerializeField]
        private ARCameraBackground cameraBackground;

        [SerializeField]
        private ARInputManager inputManager;

        [SerializeField]
        private ARSession session;

        public static bool IsMobileAR { get; private set; } = false;

        protected IEnumerator Start()
        {
            if (ARSession.state == ARSessionState.None ||
                ARSession.state == ARSessionState.CheckingAvailability)
            {
                yield return ARSession.CheckAvailability();
            }

            if (ARSession.state != ARSessionState.Unsupported)
            {
                session.enabled = true;
                if (session.descriptor != null && (session.descriptor.id.Contains("ARCore") || session.descriptor.id.Contains("ARKit")))
                {
                    IsMobileAR = true;
                    cameraBackground.enabled = true;
                    inputManager.enabled = true;
                }
            }
        }
    }
}
