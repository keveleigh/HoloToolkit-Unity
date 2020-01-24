// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    /// <summary>
    /// The Mixed Reality Toolkit's implementation of the <see cref="Microsoft.MixedReality.Toolkit.Teleport.IMixedRealityTeleportSystem"/>.
    /// </summary>
    public class MixedRealityTeleportSystem : BaseCoreSystem, IMixedRealityTeleportSystem, IMixedRealityInputHandler<float>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public MixedRealityTeleportSystem(
            IMixedRealityServiceRegistrar registrar) : base(registrar, null) // Teleport system does not use a profile
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MixedRealityTeleportSystem() : base(null) { } // Teleport system does not use a profile

        private TeleportEventData teleportEventData;

        private bool isTeleporting = false;
        private bool isProcessingTeleportRequest = false;
        private uint currentTeleportRequestSourceId = 0;
        private Vector2 currentInputPosition = Vector2.zero;

        private Vector3 targetPosition = Vector3.zero;
        private Vector3 targetRotation = Vector3.zero;

        /// <summary>
        /// Used to clean up event system when shutting down, if this system created one.
        /// </summary>
        private GameObject eventSystemReference = null;

        public bool IsTeleportEnabled { get; private set; }

        #region IMixedRealityInputHandler<float> Implementation

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<float> eventData)
        {
            // Don't process input if we've got an active teleport request in progress and this is a different source.
            if (isProcessingTeleportRequest && eventData.SourceId != currentTeleportRequestSourceId)
            {
                return;
            }

            currentInputPosition = eventData.InputData;

            if (currentInputPosition.sqrMagnitude > InputThresholdSquared)
            {
                // Get the angle of the pointer input
                float angle = Mathf.Atan2(currentInputPosition.x, currentInputPosition.y) * Mathf.Rad2Deg;

                // Offset the angle so it's 'forward' facing
                angle += angleOffset;
                PointerOrientation = angle;

                if (!TeleportRequestRaised)
                {
                    float absoluteAngle = Mathf.Abs(angle);

                    if (absoluteAngle < teleportActivationAngle)
                    {
                        TeleportRequestRaised = true;

                        CoreServices.TeleportSystem?.RaiseTeleportRequest(this, TeleportHotSpot);
                    }
                    else if (canMove)
                    {
                        // wrap the angle value.
                        if (absoluteAngle > 180f)
                        {
                            absoluteAngle = Mathf.Abs(absoluteAngle - 360f);
                        }

                        // Calculate the offset rotation angle from the 90 degree mark.
                        // Half the rotation activation angle amount to make sure the activation angle stays centered at 90.
                        float offsetRotationAngle = 90f - rotateActivationAngle;

                        // subtract it from our current angle reading
                        offsetRotationAngle = absoluteAngle - offsetRotationAngle;

                        // if it's less than zero, then we don't have activation
                        if (offsetRotationAngle > 0)
                        {
                            // check to make sure we're still under our activation threshold.
                            if (offsetRotationAngle < 2 * rotateActivationAngle)
                            {
                                canMove = false;
                                // Rotate the camera by the rotation amount.  If our angle is positive then rotate in the positive direction, otherwise in the opposite direction.
                                MixedRealityPlayspace.RotateAround(CameraCache.Main.transform.position, Vector3.up, angle >= 0.0f ? rotationAmount : -rotationAmount);
                            }
                            else // We may be trying to strafe backwards.
                            {
                                // Calculate the offset rotation angle from the 180 degree mark.
                                // Half the strafe activation angle to make sure the activation angle stays centered at 180f
                                float offsetStrafeAngle = 180f - backStrafeActivationAngle;
                                // subtract it from our current angle reading
                                offsetStrafeAngle = absoluteAngle - offsetStrafeAngle;

                                // Check to make sure we're still under our activation threshold.
                                if (offsetStrafeAngle > 0 && offsetStrafeAngle <= backStrafeActivationAngle)
                                {
                                    canMove = false;
                                    var height = MixedRealityPlayspace.Position.y;
                                    var newPosition = -CameraCache.Main.transform.forward * strafeAmount + MixedRealityPlayspace.Position;
                                    newPosition.y = height;
                                    MixedRealityPlayspace.Position = newPosition;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (!canTeleport && !TeleportRequestRaised)
                {
                    // Reset the move flag when the user stops moving the joystick
                    // but hasn't yet started teleport request.
                    canMove = true;
                }

                if (canTeleport)
                {
                    canTeleport = false;
                    TeleportRequestRaised = false;

                    if (TeleportSurfaceResult == TeleportSurfaceResult.Valid ||
                        TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
                    {
                        CoreServices.TeleportSystem?.RaiseTeleportStarted(this, TeleportHotSpot);
                    }
                }

                if (TeleportRequestRaised)
                {
                    canTeleport = false;
                    TeleportRequestRaised = false;
                    CoreServices.TeleportSystem?.RaiseTeleportCanceled(this, TeleportHotSpot);
                }
            }

            if (TeleportRequestRaised &&
                TeleportSurfaceResult == TeleportSurfaceResult.Valid ||
                TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
            {
                canTeleport = true;
            }
        }

        #endregion IMixedRealityInputHandler<float> Implementation

        #region IMixedRealityService Implementation

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Mixed Reality Teleport System";

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
            InitializeInternal();
        }

        private void InitializeInternal()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                var eventSystems = Object.FindObjectsOfType<EventSystem>();

                if (eventSystems.Length == 0)
                {
                    if (!IsInputSystemEnabled)
                    {
                        eventSystemReference = new GameObject("Event System");
                        eventSystemReference.AddComponent<EventSystem>();
                    }
                    else
                    {
                        Debug.Log("The input system didn't properly add an event system to your scene. Please make sure the input system's priority is set higher than the teleport system.");
                    }
                }
                else if (eventSystems.Length > 1)
                {
                    Debug.Log("Too many event systems in the scene. The Teleport System requires only one.");
                }
            }
#endif // UNITY_EDITOR

            teleportEventData = new TeleportEventData(EventSystem.current);
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<float>>(this);
            isEnabled = true;
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<float>>(this);
            isEnabled = false;
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            if (eventSystemReference != null)
            {
                if (!Application.isPlaying)
                {
                    Object.DestroyImmediate(eventSystemReference);
                }
                else
                {
                    Object.Destroy(eventSystemReference);
                }
            }
        }

        #endregion IMixedRealityService Implementation

        #region IEventSystemManager Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            Debug.Assert(eventData != null);
            var teleportData = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
            Debug.Assert(teleportData != null);
            Debug.Assert(!teleportData.used);

            // Process all the event listeners
            base.HandleEvent(teleportData, eventHandler);
        }

        /// <summary>
        /// Register a <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> to listen to teleport events.
        /// </summary>
        public override void Register(GameObject listener) => base.Register(listener);

        /// <summary>
        /// Unregister a <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> from listening to teleport events.
        /// </summary>
        public override void Unregister(GameObject listener) => base.Unregister(listener);

        #endregion IEventSystemManager Implementation

        #region IMixedRealityTeleportSystem Implementation

        /// <summary>
        /// Is an input system registered?
        /// </summary>
        private bool IsInputSystemEnabled => CoreServices.InputSystem != null;

        private float teleportDuration = 0.25f;

        /// <inheritdoc />
        public float TeleportDuration
        {
            get => teleportDuration;
            set
            {
                if (isProcessingTeleportRequest)
                {
                    Debug.LogWarning("Couldn't change teleport duration. Teleport in progress.");
                    return;
                }

                teleportDuration = value;
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportRequestHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportRequest(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportRequest(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
            HandleEvent(teleportEventData, OnTeleportRequestHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportStartedHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportStarted(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            if (isTeleporting)
            {
                Debug.LogError("Teleportation already in progress");
                return;
            }

            isTeleporting = true;

            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
            HandleEvent(teleportEventData, OnTeleportStartedHandler);

            ProcessTeleportationRequest(teleportEventData);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportCompletedHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportCompleted(casted);
            };

        /// <summary>
        /// Raise a teleportation completed event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target</param>
        private void RaiseTeleportComplete(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            if (!isTeleporting)
            {
                Debug.LogError("No Active Teleportation in progress.");
                return;
            }

            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
            HandleEvent(teleportEventData, OnTeleportCompletedHandler);

            isTeleporting = false;
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportCanceledHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportCanceled(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
            HandleEvent(teleportEventData, OnTeleportCanceledHandler);
        }

        #endregion IMixedRealityTeleportSystem Implementation

        private void ProcessTeleportationRequest(TeleportEventData eventData)
        {
            isProcessingTeleportRequest = true;

            targetRotation = Vector3.zero;
            var teleportPointer = eventData.Pointer as IMixedRealityTeleportPointer;
            if (teleportPointer != null)
            {
                targetRotation.y = teleportPointer.PointerOrientation;
            }
            targetPosition = eventData.Pointer.Result.Details.Point;

            if (eventData.HotSpot != null)
            {
                targetPosition = eventData.HotSpot.Position;

                if (eventData.HotSpot.OverrideTargetOrientation)
                {
                    targetRotation.y = eventData.HotSpot.TargetOrientation;
                }
            }

            float height = targetPosition.y;
            targetPosition -= CameraCache.Main.transform.position - MixedRealityPlayspace.Position;
            targetPosition.y = height;

            MixedRealityPlayspace.Position = targetPosition;
            MixedRealityPlayspace.RotateAround(
                        CameraCache.Main.transform.position, 
                        Vector3.up, 
                        targetRotation.y - CameraCache.Main.transform.eulerAngles.y);

            isProcessingTeleportRequest = false;

            // Raise complete event using the pointer and hot spot provided.
            RaiseTeleportComplete(eventData.Pointer, eventData.HotSpot);
        }
    }
}
