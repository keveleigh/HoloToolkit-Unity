﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public interface IPointer : IEqualityComparer
    {
        uint PointerId { get; }

        string PointerName { get; set; }

        IInputSource InputSourceParent { get; }

        Cursor BaseCursor { get; set; }

        ICursorModifier CursorModifier { get; set; }

        bool InteractionEnabled { get; }

        bool FocusLocked { get; set; }

        float? PointerExtent { get; set; }

        RayStep[] Rays { get; }

        LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        IFocusable FocusTarget { get; set; }

        PointerResult Result { get; set; }

        BaseRayStabilizer RayStabilizer { get; set; }

        void OnPreRaycast();

        void OnPostRaycast();

        /// <summary>
        /// Returns the position of the input source, if available.
        /// Not all input sources support positional information, and those that do may not always have it available.
        /// </summary>
        /// <param name="position">Out parameter filled with the position if available, otherwise <see cref="Vector3.zero"/>.</param>
        /// <returns>True if a position was retrieved, false if not.</returns>
        bool TryGetPointerPosition(out Vector3 position);

        /// <summary>
        /// Returns the pointing ray of the input source, if available.
        /// Not all input sources support pointing information, and those that do may not always have it available.
        /// </summary>
        /// <param name="pointingRay">Out parameter filled with the pointing ray if available.</param>
        /// <returns>True if a pointing ray was retrieved, false if not.</returns>
        bool TryGetPointingRay(out Ray pointingRay);

        /// <summary>
        /// Returns the rotation of the input source, if available.
        /// Not all input sources support rotation information, and those that do may not always have it available.
        /// </summary>
        /// <param name="rotation">Out parameter filled with the rotation if available, otherwise <see cref="Quaternion.identity"/>.</param>
        /// <returns>True if an rotation was retrieved, false if not.</returns>
        bool TryGetPointerRotation(out Quaternion rotation);
    }
}
