// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public class ControllerMappingWizard : EditorWindow
    {
        private static ControllerMappingWizard window;
        private static readonly Vector2 minWindowSize = new Vector2(500, 0);

        public static void CreateControllerMappingWindow()
        {
            if (window != null)
            {
                Debug.Log("Only one window allowed at a time");
                // Only allow one window at a time
                return;
            }

            window = CreateInstance<ControllerMappingWizard>();
            window.titleContent = new GUIContent("Create New Action Mapping");
            window.minSize = minWindowSize;
            window.Show(true);
        }

        private void OnGUI()
        {
            SerializedObject controllerMappingRepresentation = new SerializedObject(new ControllerMappingRepresentation());

            SerializedProperty controllerTypes = controllerMappingRepresentation.FindProperty("controllerTypes");
            SerializedProperty handedness = controllerMappingRepresentation.FindProperty("handedness");

            EditorGUILayout.PropertyField(controllerTypes);
            EditorGUILayout.PropertyField(handedness);
        }

        [Serializable]
        private class ControllerMappingRepresentation : UnityEngine.Object
        {
            [SerializeField]
            [Tooltip("Controller type to instantiate at runtime.")]
            [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
            private SystemType[] controllerTypes;

            [SerializeField]
            [Tooltip("The designated hand that the device is managing.")]
            private Handedness handedness;
        }
    }
}
