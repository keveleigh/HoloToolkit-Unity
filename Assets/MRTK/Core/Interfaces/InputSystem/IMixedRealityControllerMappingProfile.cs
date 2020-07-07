// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
    public interface IMixedRealityControllerMappingProfile
    {
        /// <summary>
        /// The list of controller mappings your application can use.
        /// </summary>
        MixedRealityControllerMapping[] MixedRealityControllerMappings { get; }
    }
}
