// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    internal class InteractorDebugger : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text stateOutput;

        public XRBaseInteractor Interactor { get; set; }

        private void Update()
        {
            if (stateOutput != null && Interactor != null)
            {
                stateOutput.SetText(
                    $"{Interactor.name}\n" +
                    $"Allow hover: {Interactor.allowHover}\n" +
                    $"Is hover active: {Interactor.isHoverActive}\n" +
                    $"Has hover: {Interactor.hasHover}\n\n" +
                    $"Allow select: {Interactor.allowSelect}\n" +
                    $"Is select active: {Interactor.isSelectActive}\n" +
                    $"Has selection: {Interactor.hasSelection}\n");
            }
        }
    }
}
