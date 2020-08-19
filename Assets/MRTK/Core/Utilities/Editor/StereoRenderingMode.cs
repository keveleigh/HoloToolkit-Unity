// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A collection of stereo rendering modes supported by various XR platforms.
    /// </summary>
    public enum StereoRenderingMode
    {
        /// <summary>
        /// Two render passes, one for each eye.
        /// </summary>
        MultiPass = 0,
        /// <summary>
        /// Single pass (double-wide) rendering.
        /// </summary>
        SinglePass = 1,
        /// <summary>
        /// Single pass instanced rendering.
        /// </summary>
        Instancing = 2
    }
}
