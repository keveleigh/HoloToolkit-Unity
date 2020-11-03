// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public static class ControllerBehaviors
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="mapping"></param>
        /// <param name="data"></param>
        public static void RaiseInput(
            IMixedRealityInputSource source,
            Handedness handedness,
            MixedRealityInteractionMapping mapping,
            bool data)
        {
            if (!IsExectedAxisType(mapping.AxisType, AxisType.Digital)) { return; }

            mapping.BoolData = data;
            if (!mapping.Changed) { return; }
            
            if (data)
            {
                CoreServices.InputSystem?.RaiseOnInputDown(source, handedness, mapping.MixedRealityInputAction);
            }
            else
            {
                CoreServices.InputSystem?.RaiseOnInputUp(source, handedness, mapping.MixedRealityInputAction);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="mapping"></param>
        /// <param name="data"></param>
        public static void RaiseInput(
            IMixedRealityInputSource source,
            Handedness handedness,
            MixedRealityInteractionMapping mapping,
            float data)
        {
            if (!IsExectedAxisType(mapping.AxisType, AxisType.SingleAxis)) { return; }

            mapping.FloatData = data;
            if (!mapping.Changed) { return; }

            CoreServices.InputSystem?.RaiseFloatInputChanged(source, handedness, mapping.MixedRealityInputAction, data);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="mapping"></param>
        /// <param name="data"></param>
        public static void RaiseInput(
            IMixedRealityInputSource source,
            Handedness handedness,
            MixedRealityInteractionMapping mapping,
            Vector2 data)
        {
            if (!IsExectedAxisType(mapping.AxisType, AxisType.DualAxis)) { return; }

            mapping.Vector2Data = data;
            if (!mapping.Changed) { return; }

            CoreServices.InputSystem?.RaisePositionInputChanged(source, handedness, mapping.MixedRealityInputAction, data);
        }

        // todo - ThreeDofPosition, ThreeDofRotationm, SixDof

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        private static bool IsExectedAxisType(AxisType actual, AxisType expected)
        {
            if (actual == expected) { return true; }

            Debug.LogError($"Unsupported axis type: expected {expected}");
            return false; ;
        }
    }
}
