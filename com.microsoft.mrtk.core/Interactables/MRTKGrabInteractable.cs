// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// An <see cref="XRGrabInteractable"/> with interactor filtering.
    /// </summary>
    public class MRTKGrabInteractable : XRGrabInteractable
    {
        [SerializeField]
        [Tooltip("Subtractively specifies the set of interactors allowed to select this interactable")]
        [Implements(typeof(IXRInteractor), TypeGrouping.ByNamespaceFlat, AllowAbstract = true)]
        private List<SystemInterfaceType> disabledInteractorTypes = new List<SystemInterfaceType>();

        /// <inheritdoc />
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return base.IsSelectableBy(interactor) && IsInteractorTypeValid(interactor);
        }

        /// <inheritdoc />
        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return base.IsHoverableBy(interactor) && IsInteractorTypeValid(interactor);
        }

        /// <summary>
        /// Is the given type of interactor permitted to interact with this interactable?
        /// </summary>
        private bool IsInteractorTypeValid(IXRInteractor interactor)
        {
            // Cache queried interactor type to extract from hot loop.
            Type interactorType = interactor.GetType();

            foreach (SystemInterfaceType disabledType in disabledInteractorTypes)
            {
                if (disabledType.Type.IsAssignableFrom(interactorType))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
