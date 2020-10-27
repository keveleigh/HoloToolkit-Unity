// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Controller Definition", fileName = "NewControllerDefinition", order = (int)CreateProfileMenuItemIndices.ControllerDefinition)]
    public class ControllerDefinition : ScriptableObject
    {
        [SerializeField]
        private MixedRealityControllerMapping[] controllerMappings = new MixedRealityControllerMapping[0];

        public MixedRealityControllerMapping[] MixedRealityControllerMappings => controllerMappings;

        //[SerializeField]
        //private string controllerImagePath = null;

        [SerializeField]
        [Tooltip("Controller type to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerType = null;

        /// <summary>
        /// Controller Type to instantiate at runtime.
        /// </summary>
        public SystemType ControllerType => controllerType;

        public bool TryGetControllerMapping(out MixedRealityControllerMapping controllerMapping, Handedness handedness = Handedness.None)
        {
            foreach (var mapping in controllerMappings)
            {
                if ((mapping.Handedness & handedness) == handedness)
                {
                    controllerMapping = mapping;
                    return true;
                }
            }

            controllerMapping = default(MixedRealityControllerMapping);
            return false;
        }
    }
}
