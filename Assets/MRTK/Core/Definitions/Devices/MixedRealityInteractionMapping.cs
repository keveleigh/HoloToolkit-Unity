// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Maps the capabilities of controllers, linking the Physical inputs of a controller to a Logical construct in a runtime project<para/>
    /// </summary>
    /// <remarks>
    /// One definition should exist for each physical device input, such as buttons, triggers, joysticks, dpads, and more.
    /// </remarks>
    [Serializable]
    public class MixedRealityInteractionMapping : MixedRealityInputActionMapping
    {
        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="id">Identity for mapping</param>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="axisCodeX">Optional horizontal or single axis value to get axis data from Unity's old input system.</param>
        /// <param name="axisCodeY">Optional vertical axis value to get axis data from Unity's old input system.</param>
        /// <param name="invertXAxis">Optional horizontal axis invert option.</param>
        /// <param name="invertYAxis">Optional vertical axis invert option.</param> 
        public MixedRealityInteractionMapping(uint id, string description, AxisType axisType, DeviceInputType inputType, string axisCodeX = "", string axisCodeY = "", bool invertXAxis = false, bool invertYAxis = false)
            : this(id, description, axisType, inputType, MixedRealityInputAction.None, KeyCode.None, axisCodeX, axisCodeY, invertXAxis, invertYAxis) { }

        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="id">Identity for mapping</param>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="keyCode">Optional KeyCode value to get input from Unity's old input system</param>
        public MixedRealityInteractionMapping(uint id, string description, AxisType axisType, DeviceInputType inputType, KeyCode keyCode)
            : this(id, description, axisType, inputType, MixedRealityInputAction.None, keyCode) { }

        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="id">Identity for mapping</param>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="inputAction">The logical MixedRealityInputAction that this input performs</param>
        /// <param name="keyCode">Optional KeyCode value to get input from Unity's old input system</param>
        /// <param name="axisCodeX">Optional horizontal or single axis value to get axis data from Unity's old input system.</param>
        /// <param name="axisCodeY">Optional vertical axis value to get axis data from Unity's old input system.</param>
        /// <param name="invertXAxis">Optional horizontal axis invert option.</param>
        /// <param name="invertYAxis">Optional vertical axis invert option.</param> 
        public MixedRealityInteractionMapping(uint id, string description, AxisType axisType, DeviceInputType inputType, MixedRealityInputAction inputAction, KeyCode keyCode = KeyCode.None, string axisCodeX = "", string axisCodeY = "", bool invertXAxis = false, bool invertYAxis = false)
            : this(description, axisType, inputType, inputAction, keyCode, axisCodeX, axisCodeY, invertXAxis, invertYAxis)
        {
            this.id = id;
        }

        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="inputAction">The logical MixedRealityInputAction that this input performs</param>
        /// <param name="keyCode">Optional KeyCode value to get input from Unity's old input system</param>
        /// <param name="axisCodeX">Optional horizontal or single axis value to get axis data from Unity's old input system.</param>
        /// <param name="axisCodeY">Optional vertical axis value to get axis data from Unity's old input system.</param>
        /// <param name="invertXAxis">Optional horizontal axis invert option.</param>
        /// <param name="invertYAxis">Optional vertical axis invert option.</param> 
        public MixedRealityInteractionMapping(string description, AxisType axisType, DeviceInputType inputType, MixedRealityInputAction inputAction, KeyCode keyCode = KeyCode.None, string axisCodeX = "", string axisCodeY = "", bool invertXAxis = false, bool invertYAxis = false)
            : base(description, axisType, inputType, inputAction)
        {
            this.keyCode = keyCode;
            this.axisCodeX = axisCodeX;
            this.axisCodeY = axisCodeY;
            this.invertXAxis = invertXAxis;
            this.invertYAxis = invertYAxis;
        }

        public MixedRealityInteractionMapping(MixedRealityInteractionMapping mixedRealityInteractionMapping)
            : this(mixedRealityInteractionMapping.id,
                   mixedRealityInteractionMapping.Description,
                   mixedRealityInteractionMapping.AxisType,
                   mixedRealityInteractionMapping.InputType,
                   mixedRealityInteractionMapping.InputAction,
                   mixedRealityInteractionMapping.keyCode,
                   mixedRealityInteractionMapping.axisCodeX,
                   mixedRealityInteractionMapping.axisCodeY,
                   mixedRealityInteractionMapping.invertXAxis,
                   mixedRealityInteractionMapping.invertYAxis) { }

        public MixedRealityInteractionMapping(MixedRealityInputActionMapping mixedRealityInputActionMapping, MixedRealityInteractionMappingLegacyInput legacyInput)
            : this(mixedRealityInputActionMapping.Description,
                   mixedRealityInputActionMapping.AxisType,
                   mixedRealityInputActionMapping.InputType,
                   mixedRealityInputActionMapping.InputAction,
                   legacyInput.KeyCode,
                   legacyInput.AxisCodeX,
                   legacyInput.AxisCodeY,
                   legacyInput.InvertXAxis,
                   legacyInput.InvertYAxis) { }

        #region Interaction Properties

        [SerializeField]
        [Tooltip("The Id assigned to the Interaction.")]
        private uint id;

        /// <summary>
        /// The Id assigned to the Interaction.
        /// </summary>
        public uint Id => id;

        /// <summary>
        /// Action to be raised to the Input Manager when the input data has changed.
        /// </summary>
        [Obsolete("MixedRealityInputAction property has been deprecated. Please use InputAction instead.")]
        public MixedRealityInputAction MixedRealityInputAction
        {
            get { return InputAction; }
            internal set { InputAction = value; }
        }

        [SerializeField]
        [Tooltip("Optional KeyCode value to get input from Unity's old input system.")]
        private KeyCode keyCode;

        /// <summary>
        /// Optional KeyCode value to get input from Unity's old input system.
        /// </summary>
        public KeyCode KeyCode => keyCode;

        [SerializeField]
        [Tooltip("Optional horizontal or single axis value to get axis data from Unity's old input system.")]
        private string axisCodeX;

        /// <summary>
        /// Optional horizontal or single axis value to get axis data from Unity's old input system.
        /// </summary>
        public string AxisCodeX => axisCodeX;

        [SerializeField]
        [Tooltip("Optional vertical axis value to get axis data from Unity's old input system.")]
        private string axisCodeY;

        /// <summary>
        /// Optional vertical axis value to get axis data from Unity's old input system.
        /// </summary>
        public string AxisCodeY => axisCodeY;

        [SerializeField]
        [Tooltip("Should the X axis be inverted?")]
        private bool invertXAxis = false;

        /// <summary>
        /// Should the X axis be inverted?
        /// </summary>
        /// <remarks>
        /// Only valid for <see cref="Microsoft.MixedReality.Toolkit.Utilities.AxisType.SingleAxis"/> and <see cref="Microsoft.MixedReality.Toolkit.Utilities.AxisType.DualAxis"/> inputs.
        /// </remarks>
        public bool InvertXAxis
        {
            get { return invertXAxis; }
            set
            {
                if (AxisType != AxisType.SingleAxis && AxisType != AxisType.DualAxis)
                {
                    Debug.LogWarning("Inverted X axis only valid for Single or Dual Axis inputs.");
                    return;
                }

                invertXAxis = value;
            }
        }

        [SerializeField]
        [Tooltip("Should the Y axis be inverted?")]
        private bool invertYAxis = false;

        /// <summary>
        /// Should the Y axis be inverted?
        /// </summary>
        /// <remarks>
        /// Only valid for <see cref="Microsoft.MixedReality.Toolkit.Utilities.AxisType.DualAxis"/> inputs.
        /// </remarks>
        public bool InvertYAxis
        {
            get { return invertYAxis; }
            set
            {
                if (AxisType != AxisType.DualAxis)
                {
                    Debug.LogWarning("Inverted Y axis only valid for Dual Axis inputs.");
                    return;
                }

                invertYAxis = value;
            }
        }

        private bool changed;

        /// <summary>
        /// Has the value changed since the last reading.
        /// </summary>
        public bool Changed
        {
            get
            {
                bool returnValue = changed;

                if (changed)
                {
                    changed = false;
                }

                return returnValue;
            }
            private set
            {
                changed = value;
            }
        }

        #endregion Interaction Properties

        #region Definition Data Items

        private object rawData = null;

        private bool boolData = false;

        private float floatData = 0.0f;

        private Vector2 vector2Data = Vector2.zero;

        private Vector3 positionData = Vector3.zero;

        private Quaternion rotationData = Quaternion.identity;

        private MixedRealityPose poseData = MixedRealityPose.ZeroIdentity;

        #endregion Definition Data Items

        #region Data Properties

        /// <summary>
        /// The Raw (object) data value.
        /// </summary>
        /// <remarks>Only supported for a Raw mapping axis type</remarks>
        public object RawData
        {
            get
            {
                return rawData;
            }

            set
            {
                if (AxisType != AxisType.Raw)
                {
                    Debug.LogError($"SetRawValue is only valid for AxisType.Raw InteractionMappings\nPlease check the {InputType} mapping for the current controller");
                }

                Changed = rawData != value;
                rawData = value;
            }
        }

        /// <summary>
        /// The Bool data value.
        /// </summary>
        /// <remarks>Only supported for a Digital mapping axis type</remarks>
        public bool BoolData
        {
            get
            {
                return boolData;
            }

            set
            {
                if (AxisType != AxisType.Digital && AxisType != AxisType.SingleAxis && AxisType != AxisType.DualAxis)
                {
                    Debug.LogError($"SetBoolValue is only valid for AxisType.Digital, AxisType.SingleAxis, or AxisType.DualAxis InteractionMappings\nPlease check the {InputType} mapping for the current controller");
                }

                Changed = boolData != value;
                boolData = value;
            }
        }

        /// <summary>
        /// The Float data value.
        /// </summary>
        /// <remarks>Only supported for a SingleAxis mapping axis type</remarks>
        public float FloatData
        {
            get
            {
                return floatData;
            }

            set
            {
                if (AxisType != AxisType.SingleAxis)
                {
                    Debug.LogError($"SetFloatValue is only valid for AxisType.SingleAxis InteractionMappings\nPlease check the {InputType} mapping for the current controller");
                }

                if (invertXAxis)
                {
                    Changed = !floatData.Equals(value * -1f);
                    floatData = value * -1f;
                }
                else
                {
                    Changed = !floatData.Equals(value);
                    floatData = value;
                }
            }
        }

        /// <summary>
        /// The Vector2 data value.
        /// </summary>
        /// <remarks>Only supported for a DualAxis mapping axis type</remarks>
        public Vector2 Vector2Data
        {
            get
            {
                return vector2Data;
            }

            set
            {
                if (AxisType != AxisType.DualAxis)
                {
                    Debug.LogError($"SetVector2Value is only valid for AxisType.DualAxis InteractionMappings\nPlease check the {InputType} mapping for the current controller");
                }

                Vector2 newValue = value * new Vector2(invertXAxis ? -1f : 1f, invertYAxis ? -1f : 1f);
                Changed = vector2Data != newValue;
                vector2Data = newValue;
            }
        }

        /// <summary>
        /// The ThreeDof Vector3 Position data value.
        /// </summary>
        /// <remarks>Only supported for a ThreeDof mapping axis type</remarks>
        public Vector3 PositionData
        {
            get
            {
                return positionData;
            }

            set
            {
                if (AxisType != AxisType.ThreeDofPosition)
                {
                    {
                        Debug.LogError($"SetPositionValue is only valid for AxisType.ThreeDoFPosition InteractionMappings\nPlease check the {InputType} mapping for the current controller");
                    }
                }

                Changed = positionData != value;
                positionData = value;
            }
        }

        /// <summary>
        /// The ThreeDof Quaternion Rotation data value.
        /// </summary>
        /// <remarks>Only supported for a ThreeDof mapping axis type</remarks>
        public Quaternion RotationData
        {
            get
            {
                return rotationData;
            }

            set
            {
                if (AxisType != AxisType.ThreeDofRotation)
                {
                    Debug.LogError($"SetRotationValue is only valid for AxisType.ThreeDoFRotation InteractionMappings\nPlease check the {InputType} mapping for the current controller");
                }

                Changed = rotationData != value;
                rotationData = value;
            }
        }

        /// <summary>
        /// The Pose data value.
        /// </summary>
        /// <remarks>Only supported for a SixDof mapping axis type</remarks>
        public MixedRealityPose PoseData
        {
            get
            {
                return poseData;
            }
            set
            {
                if (AxisType != AxisType.SixDof)
                {
                    Debug.LogError($"SetPoseValue is only valid for AxisType.SixDoF InteractionMappings\nPlease check the {InputType} mapping for the current controller");
                }

                Changed = poseData != value;

                poseData = value;
                positionData = poseData.Position;
                rotationData = poseData.Rotation;
            }
        }

        #endregion Data Properties
    }
}