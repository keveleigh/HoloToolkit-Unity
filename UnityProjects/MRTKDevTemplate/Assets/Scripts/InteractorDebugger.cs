// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    internal class InteractorDebugger : MonoBehaviour
    {
        [SerializeField]
        private XRBaseInteractor[] interactors = Array.Empty<XRBaseInteractor>();

        [SerializeField]
        private GameObject gizmoPrefab = null;

        private List<GameObject> gizmos = new List<GameObject>(4);

        private void OnEnable()
        {
            if (gizmoPrefab != null)
            {
                foreach (XRBaseInteractor interactor in interactors)
                {
                    if (interactor != null)
                    {
                        gizmos.Add(Instantiate(gizmoPrefab, interactor.transform));
                        GameObject attachTransformGizmo = Instantiate(gizmoPrefab, interactor.attachTransform);
                        attachTransformGizmo.transform.localScale *= 0.5f;
                        gizmos.Add(attachTransformGizmo);
                    }
                }
            }
        }

        private void OnDisable()
        {
            foreach (GameObject gizmo in gizmos)
            {
                Destroy(gizmo);
            }

            gizmos.Clear();
        }
    }
}
