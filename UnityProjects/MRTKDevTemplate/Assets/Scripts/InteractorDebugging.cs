// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    internal class InteractorDebugging : MonoBehaviour
    {
        [SerializeField]
        private XRBaseInteractor[] interactors = Array.Empty<XRBaseInteractor>();

        [SerializeField]
        private InteractorDebugger gizmoPrefab = null;

        [SerializeField]
        private bool visualizeTransform = true;

        [SerializeField]
        private bool visualizeAttachTransform = true;

        private List<InteractorDebugger> gizmos = new List<InteractorDebugger>(4);

        private void OnEnable()
        {
            if (gizmoPrefab != null)
            {
                foreach (XRBaseInteractor interactor in interactors)
                {
                    if (interactor != null)
                    {
                        if (visualizeTransform)
                        {
                            InteractorDebugger transformGizmo = Instantiate(gizmoPrefab, interactor.transform);
                            transformGizmo.Interactor = interactor;
                            gizmos.Add(transformGizmo);
                        }

                        if (visualizeAttachTransform)
                        {
                            InteractorDebugger attachTransformGizmo = Instantiate(gizmoPrefab, interactor.attachTransform);
                            attachTransformGizmo.transform.localScale *= 0.5f;
                            gizmos.Add(attachTransformGizmo);
                        }
                    }
                }
            }
        }

        private void OnDisable()
        {
            foreach (InteractorDebugger gizmo in gizmos)
            {
                Destroy(gizmo.gameObject);
            }

            gizmos.Clear();
        }
    }
}
