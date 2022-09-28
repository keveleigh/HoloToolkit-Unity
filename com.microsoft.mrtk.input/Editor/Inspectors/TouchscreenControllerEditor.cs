// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    /// <summary>
    /// Custom editor for a <see cref="TouchscreenController"/>.
    /// </summary>
    [CustomEditor(typeof(TouchscreenController), true), CanEditMultipleObjects]
    internal class TouchscreenControllerEditor : XRBaseControllerEditor
    {
        private SerializedProperty touchPointActionProperty;
        private SerializedProperty isTouchedActionProperty;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            touchPointActionProperty = serializedObject.FindProperty(nameof(touchPointActionProperty));
            isTouchedActionProperty = serializedObject.FindProperty(nameof(isTouchedActionProperty));
        }

        /// <inheritdoc />
        protected override void DrawTrackingConfiguration()
        {
            base.DrawTrackingConfiguration();
            EditorGUILayout.PropertyField(touchPointActionProperty);
        }

        /// <inheritdoc />
        protected override void DrawInputConfiguration()
        {
            base.DrawInputConfiguration();
            EditorGUILayout.PropertyField(isTouchedActionProperty);
        }
    }
}
