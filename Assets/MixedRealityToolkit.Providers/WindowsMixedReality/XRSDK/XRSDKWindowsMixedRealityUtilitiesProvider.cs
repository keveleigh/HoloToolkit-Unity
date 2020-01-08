// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

#if WMR_ENABLED
using UnityEngine.XR.WindowsMR;
#endif // WMR_ENABLED

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    public class XRSDKWindowsMixedRealityUtilitiesProvider : IWindowsMixedRealityUtilitiesProvider
    {
        /// <inheritdoc />
        IntPtr IWindowsMixedRealityUtilitiesProvider.ISpatialCoordinateSystemPtr =>
#if WMR_ENABLED
            WindowsMREnvironment.OriginSpatialCoordinateSystem;
#else
            IntPtr.Zero;
#endif

        /// <inheritdoc />
        IntPtr IWindowsMixedRealityUtilitiesProvider.IHolographicFramePtr
        {
            get
            {
                // NOTE: Currently unable to access HolographicFrame in XR SDK.
                return IntPtr.Zero;
            }
        }
    }
}
