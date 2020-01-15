// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Represents a subclass of a Unity assembly definition (asmdef) file.
    /// </summary>
    [Serializable]
    public struct VersionDefine : IEquatable<VersionDefine>
    {
        public VersionDefine(string name, string expression, string define)
        {
            this.name = name;
            this.expression = expression;
            this.define = define;
        }

        [SerializeField]
        private string name;

        [SerializeField]
        private string expression;

        [SerializeField]
        private string define;

        bool IEquatable<VersionDefine>.Equals(VersionDefine other)
        {
            return name.Equals(other.name) &&
                expression.Equals(other.expression) &&
                define.Equals(other.define);
        }
    }
}
