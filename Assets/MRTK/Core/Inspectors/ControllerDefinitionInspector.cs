// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(ControllerDefinition))]
    public class ControllerDefinitionInspector : UnityEditor.Editor
    {
        private static readonly GUIContent ControllerAddButtonContent = new GUIContent("+ Add a New Controller Mapping");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Controller Mapping");

        private static ControllerDefinition thisControllerDefinition;

        private SerializedProperty controllerMappingsList;
        private SerializedProperty controllerImagePath;
        private SerializedProperty controllerType;

        protected void OnEnable()
        {
            controllerMappingsList = serializedObject.FindProperty("controllerMappings");
            controllerImagePath = serializedObject.FindProperty("controllerImagePath");
            controllerType = serializedObject.FindProperty("controllerType");

            thisControllerDefinition = target as ControllerDefinition;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(controllerType);
            EditorGUILayout.PropertyField(controllerImagePath);

            if (MixedRealityEditorUtility.RenderIndentedButton(ControllerAddButtonContent, EditorStyles.miniButton))
            {
                ControllerMappingWizard.CreateControllerMappingWindow();
                return;

                var index = controllerMappingsList.arraySize;
                controllerMappingsList.InsertArrayElementAtIndex(index);
                var elementProperty = controllerMappingsList.GetArrayElementAtIndex(index);
                SerializedProperty handednessProperty = elementProperty.FindPropertyRelative("handedness");
                handednessProperty.intValue = (int)Handedness.None;
                SerializedProperty interactionsProperty = elementProperty.FindPropertyRelative("interactions");
                interactionsProperty.ClearArray();
                serializedObject.ApplyModifiedProperties();
                thisControllerDefinition.MixedRealityControllerMappings[index].ControllerType.Type = thisControllerDefinition.ControllerType.Type;
                thisControllerDefinition.MixedRealityControllerMappings[index].SetDefaultInteractionMapping(true);
                return;
            }

            if (MixedRealityEditorUtility.RenderIndentedButton(ControllerMinusButtonContent, EditorStyles.miniButton))
            {
                controllerMappingsList.DeleteArrayElementAtIndex(controllerMappingsList.arraySize - 1);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            using (var outerVerticalScope = new GUILayout.VerticalScope())
            {
                GUILayout.HorizontalScope horizontalScope = null;

                for (int i = 0; i < thisControllerDefinition.MixedRealityControllerMappings.Length; i++)
                {
                    MixedRealityControllerMapping controllerMapping = thisControllerDefinition.MixedRealityControllerMappings[i];

                    Handedness handedness = controllerMapping.Handedness;

                    var controllerMappingProperty = controllerMappingsList.GetArrayElementAtIndex(i);
                    var handednessProperty = controllerMappingProperty.FindPropertyRelative("handedness");

                    string controllerTitle = thisControllerDefinition.MixedRealityControllerMappings[i].Description;
                    var interactionsProperty = controllerMappingProperty.FindPropertyRelative("interactions");
                    if (handedness != Handedness.Right)
                    {
                        if (horizontalScope != null) { horizontalScope.Dispose(); horizontalScope = null; }
                        horizontalScope = new GUILayout.HorizontalScope();
                    }

                    GUIContent buttonContent = null;

                    if (!string.IsNullOrWhiteSpace(controllerImagePath.stringValue))
                    {
                        buttonContent = new GUIContent(controllerTitle, ControllerMappingLibrary.GetControllerTextureScaled(controllerImagePath.stringValue, handedness));
                    }
                    else
                    {
                        buttonContent = new GUIContent(controllerTitle, ControllerMappingLibrary.GetGenericControllerTextureScaled());
                    }

                    if (GUILayout.Button(buttonContent, MixedRealityStylesUtility.ControllerButtonStyle, GUILayout.Height(128f), GUILayout.MinWidth(32f), GUILayout.ExpandWidth(true)))
                    {
                        ControllerPopupWindow.Show(controllerMapping, interactionsProperty, handedness);
                    }
                }

                if (horizontalScope != null) { horizontalScope.Dispose(); horizontalScope = null; }
            }
        }
    }
}
