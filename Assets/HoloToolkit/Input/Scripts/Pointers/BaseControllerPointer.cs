// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Base Pointer class for pointers that exist in the scene as GameObjects.
    /// </summary>
    public abstract class BaseControllerPointer : AttachToController, IPointingSource
    {
        [Header("Cursor")]
        [SerializeField]
        protected GameObject CursorPrefab;

        [Header("Interaction")]
        [SerializeField]
        private bool interactionEnabled = true;

        [SerializeField]
        [Range(0f, 360f)]
        protected float CurrentPointerOrientation;

        [SerializeField]
        [Range(0.5f, 50f)]
        private float extentOverride = 2f;

        [SerializeField]
        [Tooltip("Source transform for raycast origin - leave null to use default transform")]
        protected Transform RaycastOrigin;

        [SerializeField]
        private KeyCode interactionEnabledKeyCode = KeyCode.None;

        [SerializeField]
        private KeyCode activeHoldKeyCode = KeyCode.None;

#if UNITY_WSA
        [SerializeField]
        private InteractionSourcePressInfo activeHoldPressType = InteractionSourcePressInfo.Select;

        [SerializeField]
        private InteractionSourcePressInfo interactionEnabledPressType = InteractionSourcePressInfo.Select;
#endif

        [SerializeField]
        private bool interactionRequiresHold = false;

        public bool OwnAllInput { get; set; }

        [Obsolete("Will be removed in a later version. Use OnPreRaycast / OnPostRaycast instead.")]
        public void UpdatePointer()
        {
        }

        /// <summary>
        /// True if select is pressed right now
        /// </summary>
        protected bool SelectPressed = false;

        /// <summary>
        /// True if select has been pressed once since startup
        /// </summary>
        protected bool SelectPressedOnce = false;

        private bool delayPointerRegistration = true;

        /// <summary>
        /// The Y orientation of the pointer target - used for touchpad rotation and navigation
        /// </summary>
        public virtual float PointerOrientation
        {
            get
            {
                return CurrentPointerOrientation + (RaycastOrigin != null ? RaycastOrigin.eulerAngles.y : transform.eulerAngles.y);
            }
            set
            {
                CurrentPointerOrientation = value;
            }
        }

        /// <summary>
        /// The forward direction of the targeting ray
        /// </summary>
        public virtual Vector3 PointerDirection
        {
            get { return RaycastOrigin != null ? RaycastOrigin.forward : transform.forward; }
        }

        #region MonoBehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();
            SelectPressed = false;

            if (BaseCursor != null)
            {
                BaseCursor.enabled = true;
            }

            if (!delayPointerRegistration)
            {
                FocusManager.Instance.RegisterPointer(this);
            }
        }

        protected virtual void Start()
        {
            FocusManager.AssertIsInitialized();
            InputManager.AssertIsInitialized();
            Debug.Assert(InputSource != null, "This Pointer must have a Input Source Assigned");

            FocusManager.Instance.RegisterPointer(this);
            delayPointerRegistration = false;

            SetCursor();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SelectPressed = false;

            if (BaseCursor != null)
            {
                BaseCursor.enabled = false;
            }

            if (FocusManager.IsInitialized)
            {
                FocusManager.Instance.UnregisterPointer(this);
            }
        }

        protected override void OnDestroy()
        {
            if (BaseCursor != null)
            {
                Destroy(BaseCursor.gameObject);
            }

            base.OnDestroy();
        }

        #endregion  MonoBehaviour Implementation

        public void SetCursor(GameObject newCursor = null)
        {
            CursorPrefab = newCursor == null ? CursorPrefab : newCursor;

            if (CursorPrefab != null)
            {
                var cursorObj = Instantiate(CursorPrefab, transform);
                cursorObj.name = string.Format("{0}_Cursor", name);
                BaseCursor = cursorObj.GetComponent<Cursor>();
                Debug.Assert(BaseCursor != null, "Failed to load cursor");
                BaseCursor.Pointer = this;
                Debug.Assert(BaseCursor.Pointer != null, "Failed to assign cursor!");
            }
        }

        /// <summary>
        /// Call to initiate a select action for this pointer
        /// </summary>
        public virtual void OnSelectPressed()
        {
            SelectPressed = true;
            SelectPressedOnce = true;
        }

        public virtual void OnSelectReleased()
        {
            SelectPressed = false;
        }

        #region IPointingSource Implementation

        public uint InputSourceId { get; set; }

        public IInputSource InputSource { get; set; }

        public string PointerName
        {
            get { return gameObject.name; }
            set { gameObject.name = value; }
        }

        public Cursor BaseCursor { get; set; }

        public ICursorModifier CursorModifier { get; set; }

        public virtual bool InteractionEnabled
        {
            get { return interactionEnabled; }
            set { interactionEnabled = value; }
        }

        public bool FocusLocked { get; set; }

        public float? ExtentOverride
        {
            get { return extentOverride; }
            set { extentOverride = value ?? FocusManager.GlobalPointingExtent; }
        }

        [Obsolete("Will be removed in a later version. Use Rays instead.")]
        public Ray Ray { get { return Rays[0]; } }

        public RayStep[] Rays { get; protected set; }

        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        public IFocusable FocusTarget { get; set; }

        public PointerResult Result { get; set; }

        public BaseRayStabilizer RayStabilizer { get; set; }

        public virtual void OnPreRaycast() { }

        public virtual void OnPostRaycast() { }

        public bool OwnsInput(BaseEventData eventData)
        {
            return (OwnAllInput || InputIsFromSource(eventData));
        }

        public bool InputIsFromSource(BaseEventData eventData)
        {
            var inputData = (eventData as IInputSourceInfoProvider);

            return (inputData != null)
                && (inputData.InputSource == InputSource)
                && (inputData.SourceId == InputSourceId);
        }

        #region IEquality Implementation

        public static bool Equals(IPointingSource left, IPointingSource right)
        {
            return left.Equals(right);
        }

        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IPointingSource)obj);
        }

        private bool Equals(IPointingSource other)
        {
            return other != null && InputSourceId == other.InputSourceId;
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)InputSourceId;
                hashCode = (hashCode * 397) ^ (PointerName != null ? PointerName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        #endregion IPointer Implementation
    }
}
