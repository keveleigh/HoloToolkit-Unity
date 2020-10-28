// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public abstract class BaseInputSourceDefinition : IMixedRealityInputSourceDefinition
    {
        public BaseInputSourceDefinition(IMixedRealityInputSource source, Handedness handedness)
        {
            inputSource = source;
            this.handedness = handedness;
        }

        protected readonly IMixedRealityInputSource inputSource;
        protected readonly Handedness handedness;

        protected virtual MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => null;
        protected virtual MixedRealityInteractionMapping[] DefaultRightHandedInteractions => null;

        public virtual MixedRealityInteractionMapping[] DefaultInteractions
        {
            get
            {
                switch (handedness)
                {
                    case Handedness.Left:
                        return DefaultLeftHandedInteractions;
                    case Handedness.Right:
                        return DefaultRightHandedInteractions;
                    default:
                        return null;
                }
            }
        }
    }
}
