// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

#if !UNITY_2020_1_OR_NEWER
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
#endif // !UNITY_2020_1_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public static class MixedRealityOptimizeUtils
    {
        private static readonly Type rootType = typeof(IMixedRealityOptimizeUtilsProvider);
        private static readonly IMixedRealityOptimizeUtilsProvider provider = null;

        static MixedRealityOptimizeUtils()
        {
            Assembly[] searchAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var results = new List<Type>();

            Parallel.ForEach(searchAssemblies, (assembly) =>
            {
                Parallel.ForEach(assembly.GetTypes(), (type) =>
                {
                    if (type != null && type.IsClass && !type.IsAbstract && type.IsSubclassOf(rootType))
                    {
                        results.Add(type);
                    }
                });
            });

            foreach (Type type in results)
            {
                Debug.Log(type);
                if (Activator.CreateInstance(type) is IMixedRealityOptimizeUtilsProvider serviceInstance && serviceInstance.IsValid)
                {
                    provider = serviceInstance;
                    return;
                }
            }
        }

        public static bool IsOptimalRenderingPath()
        {
            if (provider != null)
            {
                return provider.IsOptimalRenderingPath;
            }

            return true;
        }

        public static void SetOptimalRenderingPath()
        {
            if (provider != null)
            {
                provider.IsOptimalRenderingPath = true;
            }
        }

        /// <summary>
        /// Checks if the project has depth buffer sharing enabled.
        /// </summary>
        /// <returns>True if the project has depth buffer sharing enabled, false otherwise.</returns>
        public static bool IsDepthBufferSharingEnabled()
        {
            if (provider != null)
            {
                return provider.DepthBufferSharingEnabled;
            }

            return true;
        }

        public static void SetDepthBufferSharing(bool enableDepthBuffer)
        {
            if (provider != null)
            {
                provider.DepthBufferSharingEnabled = enableDepthBuffer;
            }
        }

        [Obsolete("Use IsDepthBufferFormat16bit() instead.")]
        public static bool IsWMRDepthBufferFormat16bit() => IsDepthBufferFormat16bit();

        public static bool IsDepthBufferFormat16bit()
        {
            if (provider != null)
            {
                return provider.IsDepthBufferFormat16Bit;
            }

            return true;
        }

        public static void SetDepthBufferFormat(bool set16BitDepthBuffer)
        {
            if (provider != null)
            {
                provider.IsDepthBufferFormat16Bit = set16BitDepthBuffer;
            }
        }

        public static bool IsRealtimeGlobalIlluminationEnabled()
        {
            var lightmapSettings = GetLightmapSettings();
            var property = lightmapSettings?.FindProperty("m_GISettings.m_EnableRealtimeLightmaps");
            return property != null && property.boolValue;
        }

        public static void SetRealtimeGlobalIlluminationEnabled(bool enabled)
        {
            var lightmapSettings = GetLightmapSettings();
            ChangeProperty(lightmapSettings, "m_GISettings.m_EnableRealtimeLightmaps", property => property.boolValue = enabled);
        }

        public static bool IsBakedGlobalIlluminationEnabled()
        {
            var lightmapSettings = GetLightmapSettings();
            var property = lightmapSettings?.FindProperty("m_GISettings.m_EnableBakedLightmaps");
            return property != null && property.boolValue;
        }

        public static void SetBakedGlobalIlluminationEnabled(bool enabled)
        {
            var lightmapSettings = GetLightmapSettings();
            ChangeProperty(lightmapSettings, "m_GISettings.m_EnableBakedLightmaps", property => property.boolValue = enabled);
        }

        public static bool IsBuildTargetOpenVR()
        {
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64;
        }

        public static bool IsBuildTargetUWP()
        {
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.WSAPlayer;
        }

        public static bool IsBuildTargetAndroid()
        {
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
        }

        public static bool IsBuildTargetIOS()
        {
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
        }

        public static void ChangeProperty(SerializedObject target, string name, Action<SerializedProperty> changer)
        {
            var prop = target.FindProperty(name);
            if (prop != null)
            {
                changer(prop);
                target.ApplyModifiedProperties();
            }
            else Debug.LogError("property not found: " + name);
        }

        public static SerializedObject GetSettingsObject(string className)
        {
            var settings = Unsupported.GetSerializedAssetInterfaceSingleton(className);
            return new SerializedObject(settings);
        }

        public static SerializedObject GetLightmapSettings()
        {
            var getLightmapSettingsMethod = typeof(LightmapEditorSettings).GetMethod("GetLightmapSettings", BindingFlags.Static | BindingFlags.NonPublic);
            var lightmapSettings = getLightmapSettingsMethod.Invoke(null, null) as UnityEngine.Object;
            return new SerializedObject(lightmapSettings);
        }
    }
}