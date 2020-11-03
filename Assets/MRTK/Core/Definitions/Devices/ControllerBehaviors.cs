// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Class implementing controller behaviors (ex: button pressing) by raising the appropriate <see cref="IMixedRealityInputSystem"/> events.
    /// </summary>
    public static class ControllerBehaviors
    {
        /// <summary>
        /// Raises digital axis (ex: on/off button) events.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="handedness">The controller's handedness.</param>
        /// <param name="mapping"><see cref="MixedRealityInteractionMapping"/> (ex: menu button) for the event.</param>
        /// <param name="data">The event data. True if the button is pressed, false if released.</param>
        /// <returns>True if the event was raised, false if not.</returns>
        /// <remarks>
        /// An event will not be raised in the following scenarios:
        ///  - an incompatbile mapping is provided
        ///  - the data for the mapping is unchanged
        /// </remarks>
        public static bool TryRaiseDigitalInput(
            IMixedRealityInputSource source,
            Handedness handedness,
            MixedRealityInteractionMapping mapping,
            bool data)
        {
            if (!IsExpectedAxisType(mapping.AxisType, AxisType.Digital)) 
            { 
                return false; 
            }

            mapping.BoolData = data;
            if (!mapping.Changed) 
            { 
                return false; 
            }
            
            if (data)
            {
                CoreServices.InputSystem?.RaiseOnInputDown(source, handedness, mapping.MixedRealityInputAction);
            }
            else
            {
                CoreServices.InputSystem?.RaiseOnInputUp(source, handedness, mapping.MixedRealityInputAction);
            }

            return true;
        }

        /// <summary>
        /// Raises one-dimensional axis events.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="handedness">The controller's handedness.</param>
        /// <param name="mapping"><see cref="MixedRealityInteractionMapping"/> (ex: thumbstick x-axis) for the event.</param>
        /// <param name="data">The event data.</param>
        /// <returns>True if the event was raised, false if not.</returns>
        /// <remarks>
        /// An event will not be raised in the following scenarios:
        ///  - an incompatbile mapping is provided
        ///  - the data for the mapping is unchanged
        /// </remarks>
        public static bool TryRaiseAxisInput(
            IMixedRealityInputSource source,
            Handedness handedness,
            MixedRealityInteractionMapping mapping,
            float data)
        {
            if (!IsExpectedAxisType(mapping.AxisType, AxisType.SingleAxis)) 
            { 
                return false;
            }

            mapping.FloatData = data;
            if (!mapping.Changed) 
            { 
                return false;
            }

            CoreServices.InputSystem?.RaiseFloatInputChanged(source, handedness, mapping.MixedRealityInputAction, data);

            return true;
        }

        /// <summary>
        /// Raises two-dimensional position events.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="handedness">The controller's handedness.</param>
        /// <param name="mapping"><see cref="MixedRealityInteractionMapping"/> (ex: touchpad position) for the event.</param>
        /// <param name="data">The event data.</param>
        /// <returns>True if the event was raised, false if not.</returns>
        /// <remarks>
        /// An event will not be raised in the following scenarios:
        ///  - an incompatbile mapping is provided
        ///  - the data for the mapping is unchanged
        /// </remarks>
        public static bool TryRaisePositionInput(
            IMixedRealityInputSource source,
            Handedness handedness,
            MixedRealityInteractionMapping mapping,
            Vector2 data)
        {
            if (!IsExpectedAxisType(mapping.AxisType, AxisType.DualAxis))
            { 
                return false;
            }

            mapping.Vector2Data = data;
            if (!mapping.Changed)
            {
                return false;
            }

            CoreServices.InputSystem?.RaisePositionInputChanged(source, handedness, mapping.MixedRealityInputAction, data);

            return true;
        }

        /// <summary>
        /// Raises three-dimensional position events.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="handedness">The controller's handedness.</param>
        /// <param name="mapping"><see cref="MixedRealityInteractionMapping"/> (ex: grip position) for the event.</param>
        /// <param name="data">The event data.</param>
        /// <returns>True if the event was raised, false if not.</returns>
        /// <remarks>
        /// An event will not be raised in the following scenarios:
        ///  - an incompatbile mapping is provided
        ///  - the data for the mapping is unchanged
        /// </remarks>
        public static bool TryRaisePositionInput(
            IMixedRealityInputSource source,
            Handedness handedness,
            MixedRealityInteractionMapping mapping,
            Vector3 data)
        {
            if (!IsExpectedAxisType(mapping.AxisType, AxisType.ThreeDofPosition))
            { 
                return false;
            }

            mapping.PositionData = data;
            if (!mapping.Changed)
            { 
                return false;
            }

            CoreServices.InputSystem?.RaisePositionInputChanged(source, handedness, mapping.MixedRealityInputAction, data);

            return true;
        }

        /// <summary>
        /// Raises three-dimensional rotation events.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="handedness">The controller's handedness.</param>
        /// <param name="mapping"><see cref="MixedRealityInteractionMapping"/> (ex: gruip rotation) for the event.</param>
        /// <param name="data">The event data.</param>
        /// <returns>True if the event was raised, false if not.</returns>
        /// <remarks>
        /// An event will not be raised in the following scenarios:
        ///  - an incompatbile mapping is provided
        ///  - the data for the mapping is unchanged
        /// </remarks>
        public static bool TryRaiseRotationInput(
            IMixedRealityInputSource source,
            Handedness handedness,
            MixedRealityInteractionMapping mapping,
            Quaternion data)
        {
            if (!IsExpectedAxisType(mapping.AxisType, AxisType.ThreeDofRotation))
            { 
                return false;
            }

            mapping.RotationData = data;
            if (!mapping.Changed)
            { 
                return false;
            }

            CoreServices.InputSystem?.RaiseRotationInputChanged(source, handedness, mapping.MixedRealityInputAction, data);

            return true;
        }

        /// <summary>
        /// Raises six-dimensional pose events.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="handedness">The controller's handedness.</param>
        /// <param name="mapping"><see cref="MixedRealityInteractionMapping"/> (ex: grip pose) for the event.</param>
        /// <param name="data">The event data.</param>
        /// <returns>True if the event was raised, false if not.</returns>
        /// <remarks>
        /// An event will not be raised in the following scenarios:
        ///  - an incompatbile mapping is provided
        ///  - the data for the mapping is unchanged
        /// </remarks>
        public static bool TryRaisePoseInput(
            IMixedRealityInputSource source,
            Handedness handedness,
            MixedRealityInteractionMapping mapping,
            MixedRealityPose data)
        {
            if (!IsExpectedAxisType(mapping.AxisType, AxisType.SixDof))
            { 
                return false;
            }

            mapping.PoseData = data;
            if (!mapping.Changed)
            { 
                return false;
            }

            CoreServices.InputSystem?.RaisePoseInputChanged(source, handedness, mapping.MixedRealityInputAction, data);

            return true;
        }

        /// <summary>
        /// Checks to see if the actual <see cref="AxisType"/> matches expectations.
        /// </summary>
        /// <param name="actual">The type of axis received.</param>
        /// <param name="expected">The type of axis expected.</param>
        /// <returns>True if the <see cref="AxisType"/> values match. False if the values do not match.</returns>
        private static bool IsExpectedAxisType(AxisType actual, AxisType expected)
        {
            if (actual == expected) { return true; }

            Debug.LogError($"Unsupported axis type: expected {expected}");
            return false;
        }
    }
}
