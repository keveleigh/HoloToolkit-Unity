// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public interface IMixedRealityOptimizeUtilsProvider
    {
        /// <summary>
        /// If this provider is valid for the current build target and device settings.
        /// </summary>
        bool IsValid { get; }

        bool IsOptimalRenderingPath { get; set; }

        /// <summary>
        /// Checks if the project has depth buffer sharing enabled.
        /// </summary>
        bool DepthBufferSharingEnabled { get; set; }

        /// <summary>
        /// If the depth buffer is set to 16 bit or not.
        /// </summary>
        bool IsDepthBufferFormat16Bit { get; set; }
    }
}
