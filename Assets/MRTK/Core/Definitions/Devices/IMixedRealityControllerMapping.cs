// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Used to define a controller or other input device's physical buttons, and other attributes.
    /// </summary>
    public interface IMixedRealityControllerMapping
    {
        /// <summary>
        /// Description of the Device.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Controller Type to instantiate at runtime.
        /// </summary>
        SystemType ControllerType { get; }

        /// <summary>
        /// The designated hand that the device is managing.
        /// </summary>
        Handedness Handedness { get; }

        /// <summary>
        /// Is this controller mapping using custom interactions?
        /// </summary>
        bool HasCustomInteractionMappings { get; }

        /// <summary>
        /// Details the list of available buttons / interactions available from the device.
        /// </summary>
        MixedRealityInteractionMapping[] Interactions { get; }
    }
}