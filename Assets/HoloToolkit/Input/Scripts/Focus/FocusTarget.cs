// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Base Component for handling Focus on GameObjects.
    /// </summary>
    public class FocusTarget : MonoBehaviour, IFocusable
    {
        [SerializeField]
        [Tooltip("Does this GameObject start with Focus Enabled?")]
        private bool focusEnabled = true;

        public virtual bool FocusEnabled
        {
            get { return focusEnabled; }
            set { focusEnabled = value; }
        }

        private bool hasFocus;

        public bool HasFocus { get { return FocusEnabled && hasFocus; } }

        public virtual void OnFocusEnter()
        {
            if (focusEnabled)
            {
                hasFocus = true;
            }
        }

        public virtual void OnFocusExit()
        {

            hasFocus = false;
        }
    }
}
