// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Object containing properties about a dialog.
    /// </summary>
    public class DialogProperty
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title">The title bar string (top-most) on the dialog.</param>
        /// <param name="message">The message content string of the dialog.</param>
        /// <param name="buttonContexts">The button type(s) available on the dialog.</param>
        public DialogProperty(string title, string message, params DialogButtonContext[] buttonContexts)
        {
            Title = title;
            Message = message;
            ButtonContexts = buttonContexts;
        }

        /// <summary>
        /// The title bar string (top-most) on the dialog.
        /// </summary>
        public string Title { get; } = string.Empty;

        /// <summary>
        /// The message content string of the dialog.
        /// </summary>
        public string Message { get; } = string.Empty;

        /// <summary>
        /// Contexts for the buttons, in order of their appearance on the dialog.
        /// </summary>
        public IReadOnlyList<DialogButtonContext> ButtonContexts { get; } = null;

        /// <summary>
        /// Which button was clicked to dismiss the dialog.
        /// </summary>
        public DialogButtonContext ResultContext { get; internal set; } = default;

        /// <summary>
        /// Reference to the dialog this property applies to.
        /// </summary>
        public Dialog TargetDialog { get; internal set; }
    }
}
