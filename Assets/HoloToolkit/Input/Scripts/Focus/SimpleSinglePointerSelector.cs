// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Script shows how to create your own 'point and commit' style pointer which can steal cursor focus
    /// using a pointing ray supported motion controller.
    /// This class uses the InputSourcePointer to define the rules of stealing focus when a pointing ray is detected
    /// with a motion controller that supports pointing.
    /// </summary>
    public class SimpleSinglePointerSelector : MonoBehaviour
    {
        #region Settings

        [Tooltip("The stabilizer, if any, used to smooth out controller ray data.")]
        public BaseRayStabilizer ControllerPointerStabilizer;

        [Tooltip("The cursor, if any, which should follow the selected pointer.")]
        public Cursor Cursor;

        [Tooltip("True to search for a cursor if one isn't explicitly set.")]
        public bool SearchForCursorIfUnset = true;

        #endregion

        #region Data

        private bool started;
        private bool pointerWasChanged;

        private IPointingSource currentPointer;

        private InputSourcePointer inputSourcePointer;

        #endregion

        #region MonoBehaviour Implementation

        private void Start()
        {
            if (inputSourcePointer != null)
            {
                Instantiate(inputSourcePointer);
            }
            else
            {
                inputSourcePointer = new InputSourcePointer();
            }

            started = true;

            InputManager.AssertIsInitialized();
            FocusManager.AssertIsInitialized();
            GazeManager.AssertIsInitialized();


            ConnectBestAvailablePointer();
        }

        #endregion



        

        private void SetPointer(IPointingSource newPointer)
        {
            if (currentPointer != newPointer)
            {
                if (currentPointer != null)
                {
                    FocusManager.Instance.UnregisterPointer(currentPointer);
                }

                currentPointer = newPointer;

                if (newPointer != null)
                {
                    FocusManager.Instance.RegisterPointer(newPointer);
                }

                if (Cursor != null)
                {
                    Cursor.Pointer = newPointer;
                }
            }

            Debug.Assert(currentPointer != null, "No Pointer Set!");
        }

        private void ConnectBestAvailablePointer()
        {
            IPointingSource bestPointer = null;
            var inputSources = InputManager.Instance.DetectedInputSources;

            for (var i = 0; i < inputSources.Count; i++)
            {
                if (SupportsPointingRay(inputSources[i]))
                {
                    AttachInputSourcePointer(inputSources[i]);
                    bestPointer = inputSourcePointer;
                    break;
                }
            }

            if (bestPointer == null)
            {
                bestPointer = GazeManager.Instance;
            }

            SetPointer(bestPointer);
        }

        private void HandleInputAction(InputEventData eventData)
        {
            // TODO: robertes: Investigate how this feels. Since "Down" will often be followed by "Click", is
            //       marking the event as used actually effective in preventing unintended app input during a
            //       pointer change?

            if (SupportsPointingRay(eventData))
            {
                if (IsInputSourcePointerActive && inputSourcePointer.InputIsFromSource(eventData))
                {
                    pointerWasChanged = false;
                }
                else
                {
                    AttachInputSourcePointer(eventData);
                    SetPointer(inputSourcePointer);
                    pointerWasChanged = true;
                }
            }
            else
            {
                if (IsGazePointerActive)
                {
                    pointerWasChanged = false;
                }
                else
                {
                    // TODO: robertes: see if we can treat voice separately from the other simple committers,
                    //       so voice doesn't steal from a pointing controller. I think input Kind would need
                    //       to come through with the event data.
                    SetPointer(GazeManager.Instance);
                    pointerWasChanged = true;
                }
            }

            if (pointerWasChanged)
            {
                // Since this input resulted in a pointer change, we mark the event as used to
                // prevent it from falling through to other handlers to prevent potentially
                // unintended input from reaching handlers that aren't being pointed at by
                // the new pointer.
                eventData.Use();
            }
        }

        private bool SupportsPointingRay(BaseInputEventData eventData)
        {
            return SupportsPointingRay(eventData.InputSource, eventData.SourceId);
        }

        private bool SupportsPointingRay(InputSourceInfo source)
        {
            return SupportsPointingRay(source.InputSource, source.SourceId);
        }

        private bool SupportsPointingRay(IInputSource inputSource, uint sourceId)
        {
            return inputSource.SupportsInputInfo(sourceId, SupportedInputInfo.Pointing);
        }

        private void AttachInputSourcePointer(BaseInputEventData eventData)
        {
            AttachInputSourcePointer(eventData.InputSource, eventData.SourceId);
        }

        private void AttachInputSourcePointer(InputSourceInfo source)
        {
            AttachInputSourcePointer(source.InputSource, source.SourceId);
        }

        private void AttachInputSourcePointer(IInputSource inputSource, uint sourceId)
        {
            inputSourcePointer.InputSource = inputSource;
            inputSourcePointer.InputSourceId = sourceId;
            inputSourcePointer.RayStabilizer = ControllerPointerStabilizer;
            inputSourcePointer.OwnAllInput = false;
            inputSourcePointer.ExtentOverride = null;
            inputSourcePointer.PrioritizedLayerMasksOverride = null;
        }

        private bool IsInputSourcePointerActive
        {
            get { return ReferenceEquals(currentPointer, inputSourcePointer); }
        }

        private bool IsGazePointerActive
        {
            get { return ReferenceEquals(currentPointer, GazeManager.Instance); }
        }
    }
}
