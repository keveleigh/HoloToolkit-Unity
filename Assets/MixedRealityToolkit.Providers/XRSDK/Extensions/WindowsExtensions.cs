// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Utilities;
using Windows.UI.Input.Spatial;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.XRSDK.Windows
{
    public static class WindowsExtensions
    {
#if WINDOWS_UWP
        public static Handedness ToMRTKHandedness(this SpatialInteractionSourceHandedness handedness)
        {
            switch (handedness)
            {
                case SpatialInteractionSourceHandedness.Left:
                    return Handedness.Left;
                case SpatialInteractionSourceHandedness.Right:
                    return Handedness.Right;
                case SpatialInteractionSourceHandedness.Unspecified:
                default:
                    return Handedness.None;
            }
        }
#endif // WINDOWS_UWP
    }
}
