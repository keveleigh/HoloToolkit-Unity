// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
#endif // UNITY_EDITOR

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Class to perform configuration checks for a specified provider.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/AsmDef Configuration Checker", fileName = "ConfigurationChecker", order = 100)]
    public class AsmDefConfigurationChecker : ScriptableObject
    {
        [SerializeField]
        private string asmDefFileName = string.Empty;

        [SerializeField]
        [Tooltip("References to add for Unity 2019.")]
        private string[] references = Array.Empty<string>();

        [SerializeField]
        [Tooltip("Version defines to add for Unity 2019.")]
        private VersionDefine[] versionDefines = Array.Empty<VersionDefine>();

#if UNITY_EDITOR
        /// <summary>
        /// Updates the assembly definition to contain the appropriate references based on the Unity version.
        /// </summary>
        /// <remarks>
        /// Versions of Unity may have different factorings of components. To address this, the UpdateAsmDef
        /// method conditionally compiles for the version currently in use.
        /// To ensure proper compilation on each Unity version, the following steps are performed:
        /// - Load the Microsoft.MixedReality.Toolkit.Providers.XRSDK.WMR.asmdef file
        /// - If Unity 2018: nothing
        /// - If Unity 2019 and newer: Unity.XR.WindowsMixedReality
        /// - Save the Microsoft.MixedReality.Toolkit.Providers.XRSDK.WMR.asmdef file
        /// This will result in Unity reloading the assembly with the appropriate dependencies.
        /// </remarks>
        private void Awake()
        {
            FileInfo[] asmDefFiles = FileUtilities.FindFilesInAssets(asmDefFileName);

            if (asmDefFiles.Length == 0)
            {
                Debug.LogWarning($"Unable to locate file: {asmDefFileName}");
                return;
            }
            if (asmDefFiles.Length > 1)
            {
                Debug.LogWarning($"Multiple ({asmDefFiles.Length}) {asmDefFileName} instances found. Modifying only the first.");
            }

            AssemblyDefinition asmDef = AssemblyDefinition.Load(asmDefFiles[0].FullName);
            if (asmDef == null)
            {
                Debug.LogWarning($"Unable to load file: {asmDefFileName}");
                return;
            }

            List<string> newReferences = new List<string>();
            if (asmDef.References != null)
            {
                newReferences.AddRange(asmDef.References);
            }

            bool changed = false;

#if UNITY_2019_3_OR_NEWER
            List<VersionDefine> defines = new List<VersionDefine>();
            if (asmDef.VersionDefines != null)
            {
                defines.AddRange(asmDef.VersionDefines);
            }

            foreach (string reference in references)
            {
                if (!newReferences.Contains(reference))
                {
                    newReferences.Add(reference);
                    changed = true;
                }
            }

            foreach (VersionDefine define in versionDefines)
            {
                if (!defines.Contains(define))
                {
                    defines.Add(define);
                    changed = true;
                }
            }
#else
            foreach (string reference in references)
            {
                if (newReferences.Contains(reference))
                {
                    newReferences.Remove(reference);
                    changed = true;
                }
            }
#endif

            if (changed)
            {
                asmDef.References = newReferences.ToArray();
#if UNITY_2019_3_OR_NEWER
                asmDef.VersionDefines = defines.ToArray();
#endif // UNITY_2019_3_OR_NEWER
                asmDef.Save(asmDefFiles[0].FullName);
            }
        }
#endif // UNITY_EDITOR
    }
}
