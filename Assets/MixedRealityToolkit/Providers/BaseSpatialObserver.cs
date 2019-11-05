// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Class providing a base implementation of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface.
    /// </summary>
    public abstract class BaseSpatialObserver : BaseDataProvider<IMixedRealitySpatialAwarenessSystem>, IMixedRealitySpatialAwarenessObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the observer.</param>
        /// <param name="spatialAwarenessSystem">The <see cref="SpatialAwareness.IMixedRealitySpatialAwarenessSystem"/> to which the observer is providing data.</param>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <param name="priority">The registration priority of the data provider.</param>
        /// <param name="profile">The configuration profile for the data provider.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        protected BaseSpatialObserver(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority, 
            BaseMixedRealityProfile profile = null) : this(spatialAwarenessSystem, name, priority, profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="spatialAwarenessSystem">The <see cref="SpatialAwareness.IMixedRealitySpatialAwarenessSystem"/> to which the observer is providing data.</param>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <param name="priority">The registration priority of the data provider.</param>
        /// <param name="profile">The configuration profile for the data provider.</param>
        protected BaseSpatialObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        {
            SpatialAwarenessSystem = spatialAwarenessSystem;

            SourceId = (SpatialAwarenessSystem != null) ? SpatialAwarenessSystem.GenerateNewSourceId() : 0;
            SourceName = name;
        }

        /// <summary>
        /// The spatial awareness system that is associated with this observer.
        /// </summary>
        protected IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem { get; private set; }

        /// <summary>
        /// Creates the surface observer and handles the desired startup behavior.
        /// </summary>
        protected abstract void CreateObserver();

        /// <summary>
        /// Ensures that the surface observer has been stopped and destroyed.
        /// </summary>
        protected abstract void CleanupObserver();

        #region BaseService Implementation

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposed) { return; }

            base.Dispose(disposing);

            if (disposing)
            {
                CleanupObservationsAndObserver();
            }

            disposed = true;
        }

        #endregion BaseService Implementation

        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            return x.Equals(y);
        }

        /// <inheritdoc /> 
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealitySpatialAwarenessObserver)obj);
        }

        private bool Equals(IMixedRealitySpatialAwarenessObserver other)
        {
            return ((other != null) &&
                (SourceId == other.SourceId) &&
                string.Equals(SourceName, other.SourceName));
        }

        /// <inheritdoc />
        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc /> 
        public override int GetHashCode()
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }

        /// <inheritdoc />
        public uint SourceId { get; }

        /// <inheritdoc />
        public string SourceName { get; }

        #endregion IMixedRealityEventSource Implementation

        #region IMixedRealityDataProvider Implementation

        bool autoResume = false;

        /// <summary>
        /// Creates the observer.
        /// </summary>
        public override void Initialize()
        {
            CreateObserver();
        }

        /// <summary>
        /// Suspends the observer, clears observations, cleans up the observer, then reinitializes.
        /// </summary>
        public override void Reset()
        {
            CleanupObservationsAndObserver();
            Initialize();
        }

        /// <inheritdoc />
        public override void Enable()
        {
            // Resume iff we are not running and had been disabled while running.
            if (!IsRunning && autoResume)
            {
                Resume();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            // Remember if we are currently running when Disable is called.
            autoResume = IsRunning;

            // If we are disabled while running...
            if (IsRunning)
            {
                // Suspend the observer
                Suspend();
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            CleanupObservationsAndObserver();
        }

        #endregion IMixedRealityDataProvider Implementation

        #region IMixedRealitySpatialAwarenessObserver Implementation

        /// <inheritdoc />
        public AutoStartBehavior StartupBehavior { get; set; } = AutoStartBehavior.AutoStart;

        /// <inheritdoc />
        public int DefaultPhysicsLayer { get; } = 31;

        /// <inheritdoc />
        public bool IsRunning { get; protected set; } = false;

        /// <inheritdoc />
        public bool IsStationaryObserver { get; set; } = false;

        /// <inheritdoc />
        public Quaternion ObserverRotation { get; set; } = Quaternion.identity;

        public Vector3 ObserverOrigin { get; set; } = Vector3.zero;

        /// <inheritdoc />
        public VolumeType ObserverVolumeType { get; set; } = VolumeType.AxisAlignedCube;

        /// <inheritdoc />
        public Vector3 ObservationExtents { get; set; } = Vector3.one * 3f; // 3 meter sides / radius

        /// <inheritdoc />
        public float UpdateInterval { get; set; } = 3.5f; // 3.5 seconds

        /// <inheritdoc />
        public virtual void Resume() { }

        /// <inheritdoc />
        public virtual void Suspend() { }

        /// <inheritdoc />
        public virtual void ClearObservations() { }

        #endregion IMixedRealitySpatialAwarenessObserver Implementation

        #region Helpers

        /// <summary>
        /// Destroys all observed objects and the observer.
        /// </summary>
        private void CleanupObservationsAndObserver()
        {
            if (IsRunning)
            {
                Suspend();
            }

            // Destroys all observed objects and the observer.
            ClearObservations();
            CleanupObserver();
        }

        #endregion Helpers
    }
}
