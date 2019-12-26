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
        string Description;

        /// <summary>
        /// Controller Type to instantiate at runtime.
        /// </summary>
        SystemType ControllerType;

        public SupportedControllerType SupportedControllerType
        {
            get
            {
                if (controllerType.Type != null)
                {
                    var attr = MixedRealityControllerAttribute.Find(controllerType);
                    if (attr != null)
                    {
                        return attr.SupportedControllerType;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// The designated hand that the device is managing.
        /// </summary>
        Handedness Handedness;

        /// <summary>
        /// Is this controller mapping using custom interactions?
        /// </summary>
        bool HasCustomInteractionMappings;

        /// <summary>
        /// Details the list of available buttons / interactions available from the device.
        /// </summary>
        MixedRealityInteractionMapping[] Interactions;

        /// <summary>
        /// Sets the default interaction mapping based on the current controller type.
        /// </summary>
        internal void SetDefaultInteractionMapping(bool overwrite = false)
        {
            if (interactions == null || interactions.Length == 0 || overwrite)
            {
                MixedRealityInteractionMapping[] defaultMappings = GetDefaultInteractionMappings();

                if (defaultMappings != null)
                {
                    interactions = defaultMappings;
                }
            }
        }

        internal bool UpdateInteractionSettingsFromDefault()
        {
            bool updatedMappings = false;

            if (interactions?.Length > 0)
            {
                MixedRealityInteractionMapping[] newDefaultInteractions = GetDefaultInteractionMappings();

                if (newDefaultInteractions == null)
                {
                    return updatedMappings;
                }

                for (int i = 0; i < newDefaultInteractions.Length; i++)
                {
                    MixedRealityInteractionMapping currentMapping = interactions[i];
                    MixedRealityInteractionMapping currentDefaultMapping = newDefaultInteractions[i];

                    if (currentMapping.Id != currentDefaultMapping.Id ||
                        currentMapping.Description != currentDefaultMapping.Description ||
                        currentMapping.AxisType != currentDefaultMapping.AxisType ||
                        currentMapping.InputType != currentDefaultMapping.InputType ||
                        currentMapping.KeyCode != currentDefaultMapping.KeyCode ||
                        currentMapping.AxisCodeX != currentDefaultMapping.AxisCodeX ||
                        currentMapping.AxisCodeY != currentDefaultMapping.AxisCodeY ||
                        currentMapping.InvertXAxis != currentDefaultMapping.InvertXAxis ||
                        currentMapping.InvertYAxis != currentDefaultMapping.InvertYAxis)
                    {
                        interactions[i] = new MixedRealityInteractionMapping(currentDefaultMapping)
                        {
                            MixedRealityInputAction = currentMapping.MixedRealityInputAction
                        };

                        updatedMappings = true;
                    }
                }
            }

            return updatedMappings;
        }

        /// <summary>
        /// Synchronizes the Input Actions of the same physical controller of a different concrete type.
        /// </summary>
        internal void SynchronizeInputActions(MixedRealityInteractionMapping[] otherControllerMapping)
        {
            if (otherControllerMapping.Length != interactions.Length)
            {
                throw new ArgumentException($"otherControllerMapping length {otherControllerMapping.Length} does not match this length {interactions.Length}.");
            }

            for (int i = 0; i < interactions.Length; i++)
            {
                interactions[i].MixedRealityInputAction = otherControllerMapping[i].MixedRealityInputAction;
            }
        }
    }
}