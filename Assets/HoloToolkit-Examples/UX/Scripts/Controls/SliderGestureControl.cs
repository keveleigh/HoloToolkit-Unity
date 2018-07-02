// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Updates slider UI based on gesture input
    /// </summary>
    public class SliderGestureControl : GestureInteractiveControl
    {
        [Tooltip("The main bar of the slider, used to get the actual width of the slider")]
        public GameObject SliderBar;
        [Tooltip("The visual marker of the slider value")]
        public GameObject Knob;
        [Tooltip("The fill that represents the volume of the shader value")]
        public GameObject SliderFill;
        [Tooltip("The text representation of the slider value")]
        public TextMesh Label;

        [Tooltip("Used for centered format only, will be turned off if LeftJustified")]
        public GameObject CenteredDot;

        [Tooltip("Sends slider event information on Update")]
        public UnityEventFloat OnUpdateEvent;

        /// <summary>
        /// The value of the slider
        /// </summary>
        public float SliderValue
        {
            private set
            {
                if (sliderValue != value)
                {
                    sliderValue = value;
                    //mSliderValue = Mathf.Clamp(value, MinSliderValue, MaxSliderValue);
                    OnUpdateEvent.Invoke(sliderValue);
                }
            }
            get
            {
                return sliderValue;
            }
        }

        [Tooltip("Min numeric value to display in the slider label")]
        public float MinSliderValue = 0;

        [Tooltip("Max numeric value to display in the slider label")]
        public float MaxSliderValue = 1;

        [SerializeField]
        [Tooltip("Set the starting value for the slider here.")]
        private float sliderValue;

        [Tooltip("Switches between a left justified or centered slider")]
        public bool Centered = false;

        [Tooltip("Format the slider value and control decimal places if needed")]
        public string LabelFormat = "#.##";

        /// <summary>
        /// The range of the slider.
        /// </summary>
        private float valueSpan;

        /// <summary>
        /// The percentage value of the slider at the start of the gesture.
        /// </summary>
        private float cachedPercentageValue;

        // Cached UI values
        private Vector3 sliderHandleLocalPositionAtStart = Vector3.zero;
        private float sliderBarWidth;
        private Vector3 sliderFillScale = Vector3.one;
        private float sliderFillWidth = 0.0f;

        // Used for slider automation
        private float autoSliderTime = 0.25f;
        private float autoSliderTimerCounter = 0.5f;
        private float autoSliderValue = 0.0f;

        protected override void Awake()
        {
            base.Awake();

            if (MinSliderValue >= MaxSliderValue || SliderValue > MaxSliderValue || SliderValue < MinSliderValue)
            {
                Debug.LogError("Your SliderGestureControl has a min value that's greater than or equal to its max value or a starting value outside the min/max range.");
                Destroy(this);
                return;
            }

            if (Centered && MinSliderValue != -MaxSliderValue)
            {
                Debug.LogError("A centered SliderGestureControl requires that the min and max values have the same absolute value, one positive and one negative.");
                Destroy(this);
                return;
            }
            
            Quaternion initialRotation = SliderBar.transform.rotation;

            // with some better math below, I may be able to avoid rotating to get the proper size of the component

            SliderBar.transform.rotation = Quaternion.identity;

            // set the width of the slider 
            sliderBarWidth = SliderBar.transform.InverseTransformVector(SliderBar.GetComponent<Renderer>().bounds.size).x;

            if (Knob != null)
            {
                // Cache the slider handle's initial Z value.
                sliderHandleLocalPositionAtStart.z = Knob.transform.localPosition.z;
            }

            if (SliderFill != null)
            {
                sliderFillScale = SliderFill.transform.localScale;
                sliderFillWidth = SliderFill.transform.InverseTransformVector(SliderFill.GetComponent<Renderer>().bounds.size).x;
            }

            if (CenteredDot != null && !Centered)
            {
                CenteredDot.SetActive(false);
            }

            SliderBar.transform.rotation = initialRotation;

            valueSpan = MaxSliderValue - MinSliderValue;
            cachedPercentageValue = (SliderValue - MinSliderValue) / valueSpan;
            UpdateVisuals(cachedPercentageValue);

            if (GestureData == GestureDataType.Aligned)
            {
                AlignmentVector = SliderBar.transform.right;
            }
        }

        public override void ManipulationUpdate(Vector3 startGesturePosition, Vector3 currentGesturePosition, Vector3 startHeadOrigin, Vector3 startHeadRay, GestureInteractive.GestureManipulationState gestureState)
        {
            if (GestureData == GestureDataType.Aligned && AlignmentVector != SliderBar.transform.right)
            {
                AlignmentVector = SliderBar.transform.right;
            }

            base.ManipulationUpdate(startGesturePosition, currentGesturePosition, startHeadOrigin, startHeadRay, gestureState);

            // get the current delta
            float delta = (CurrentDistance > 0) ? CurrentPercentage : -CurrentPercentage;

            // combine the delta with the current slider position so the slider does not start over every time
            float newPercentageValue = Mathf.Clamp01(delta + cachedPercentageValue);

            UpdateSliderValue(newPercentageValue);

            UpdateVisuals(newPercentageValue);

            if (gestureState == GestureInteractive.GestureManipulationState.None)
            {
                // gesture ended - cache the current delta
                cachedPercentageValue = newPercentageValue;
            }
        }

        /// <summary>
        /// allows the slider to be automated or triggered by a key word
        /// </summary>
        /// <param name="gestureValue"></param>
        public override void SetGestureValue(int gestureValue)
        {
            //base.SetGestureValue(gestureValue);

            if (GestureStarted)
            {
                return;
            }

            switch (gestureValue)
            {
                case 0:
                    autoSliderValue = 0;
                    break;
                case 1:
                    autoSliderValue = 0.5f;
                    break;
                case 2:
                    autoSliderValue = 1;
                    break;
            }
            autoSliderTimerCounter = 0;
        }

        /// <summary>
        /// set the distance of the slider
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetSpan(float min, float max)
        {
            valueSpan = max - min;
            MaxSliderValue = max;
            MinSliderValue = min;
        }

        /// <summary>
        /// override the slider value
        /// </summary>
        /// <param name="value"></param>
        public void SetSliderValue(float value)
        {
            if (GestureStarted)
            {
                return;
            }

            sliderValue = Mathf.Clamp(value, MinSliderValue, MaxSliderValue);
            cachedPercentageValue = (SliderValue - MinSliderValue) / valueSpan;
            UpdateVisuals(cachedPercentageValue);
        }

        // update visuals
        private void UpdateVisuals(float newPercentageValue)
        {
            // TODO: Add snapping!

            if (Knob != null)
            {
                // set the knob position
                Knob.transform.localPosition = sliderHandleLocalPositionAtStart + Vector3.right * sliderBarWidth * (newPercentageValue - 0.5f);
            }

            // set the fill scale and position
            if (SliderFill != null)
            {
                Vector3 scale = sliderFillScale;
                scale.x = sliderFillScale.x * newPercentageValue;

                Vector3 position = Vector3.left * (sliderFillWidth * 0.5f - sliderFillWidth * newPercentageValue * 0.5f); // left justified

                if (Centered)
                {
                    if (SliderValue < 0)
                    {
                        position = Vector3.left * (sliderFillWidth * 0.5f - sliderFillWidth * 0.5f * (newPercentageValue + 0.5f)); // pinned to center, going left
                        scale.x = sliderFillScale.x * (1 - newPercentageValue / 0.5f) * 0.5f;
                    }
                    else
                    {
                        position = Vector3.right * ((sliderFillWidth * 0.5f * (newPercentageValue - 0.5f))); // pinned to center, going right
                        scale.x = sliderFillScale.x * ((newPercentageValue - 0.5f) / 0.5f) * 0.5f;
                    }
                }

                SliderFill.transform.localScale = scale;
                SliderFill.transform.localPosition = position;
            }

            // set the label
            if (Label != null)
            {
                if (LabelFormat.IndexOf('.') > -1)
                {
                    Label.text = SliderValue.ToString(LabelFormat);
                }
                else
                {
                    Label.text = Mathf.Round(SliderValue).ToString(LabelFormat);
                }
            }
        }

        /// <summary>
        /// Handle automation
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (autoSliderTimerCounter < autoSliderTime)
            {
                if (GestureStarted)
                {
                    autoSliderTimerCounter = autoSliderTime;
                    return;
                }

                autoSliderTimerCounter += Time.deltaTime;
                if (autoSliderTimerCounter >= autoSliderTime)
                {
                    autoSliderTimerCounter = autoSliderTime;
                    cachedPercentageValue = autoSliderValue;
                }

                float newPercentageValue = (autoSliderValue - cachedPercentageValue) * autoSliderTimerCounter / autoSliderTime + cachedPercentageValue;

                UpdateSliderValue(newPercentageValue);
                UpdateVisuals(newPercentageValue);
            }
        }

        private void UpdateSliderValue(float newPercentageValue)
        {
            if (!Centered)
            {
                SliderValue = newPercentageValue * valueSpan + MinSliderValue;
            }
            else
            {
                SliderValue = newPercentageValue * valueSpan - valueSpan * 0.5f;
            }
        }
    }
}
