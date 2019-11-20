﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.WindowsMR;

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        (SupportedPlatforms)(-1),
        "XR SDK Spatial Mesh Observer",
        "Profiles/DefaultMixedRealitySpatialAwarenessMeshObserverProfile.asset",
        "MixedRealityToolkit.SDK")]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.html")]
    public class XRSDKSpatialMeshObserver :
        BaseSpatialMeshObserver,
        IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public XRSDKSpatialMeshObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        { }

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            if (capability != MixedRealityCapability.SpatialAwarenessMesh)
            {
                return false;
            }

            var descriptors = new List<XRMeshSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(descriptors);

            return descriptors.Count > 0;
        }

        #endregion IMixedRealityCapabilityCheck Implementation

        #region IMixedRealityDataProvider implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            meshEventData = new MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>(EventSystem.current);

            CreateObserver();

            if (observer == null)
            {
                Debug.Log("Observer is null :(");
                return;
            }

            // Apply the initial observer volume settings.
            ConfigureObserverVolume();
        }

        /// <inheritdoc />
        public override void Reset()
        {
            CleanupObserver();
            Initialize();
        }

        /// <inheritdoc />
        public override void Update()
        {
            UpdateObserver();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            CleanupObserver();
        }

        #endregion IMixedRealityDataProvider implementation

        #region IMixedRealitySpatialAwarenessObserver implementation

        /// <summary>
        /// The XRMeshSubsystem providing the spatial data.
        /// </summary>
        private XRMeshSubsystem observer = null;

        /// <summary>
        /// A queue of TrackableId that need their meshes created (or updated).
        /// </summary>
        private readonly Queue<MeshId> meshWorkQueue = new Queue<MeshId>();

        private readonly List<MeshInfo> meshInfos = new List<MeshInfo>();

        /// <summary> 
        /// To prevent too many meshes from being generated at the same time, we will 
        /// only request one mesh to be created at a time.  This variable will track 
        /// if a mesh creation request is in flight. 
        /// </summary> 
        private SpatialAwarenessMeshObject outstandingMeshObject = null;

        /// <summary>
        /// When surfaces are replaced or removed, rather than destroying them, we'll keep
        /// one as a spare for use in outstanding mesh requests. That way, we'll have fewer
        /// game object create/destroy cycles, which should help performance.
        /// </summary>
        protected SpatialAwarenessMeshObject spareMeshObject = null;

        /// <summary>
        /// The time at which the surface observer was last asked for updated data.
        /// </summary>
        private float lastUpdated = 0;

        /// <inheritdoc />
        protected override int LookupTriangleDensity(SpatialAwarenessMeshLevelOfDetail levelOfDetail)
        {
            // For non - custom levels, the enum value is the appropriate triangles per cubic meter.
            int level = (int)levelOfDetail;
            observer.meshDensity = level / (float)SpatialAwarenessMeshLevelOfDetail.Fine; // For now, map Coarse to 0.0 and Fine to 1.0
            return level;
        }

        /// <inheritdoc/>
        public override void Resume()
        {
            if (IsRunning)
            {
                Debug.LogWarning("The XR SDK spatial observer is currently running.");
                return;
            }

            // We want the first update immediately.
            lastUpdated = 0;

            // UpdateObserver keys off of this value to start observing.
            IsRunning = true;
        }

        /// <inheritdoc/>
        public override void Suspend()
        {
            if (!IsRunning)
            {
                Debug.LogWarning("The XR SDK spatial observer is currently stopped.");
                return;
            }

            // UpdateObserver keys off of this value to stop observing.
            IsRunning = false;

            // Clear any pending work.
            meshWorkQueue.Clear();
        }

        /// <summary>
        /// Creates the XRMeshSubsystem and handles the desired startup behavior.
        /// </summary>
        private void CreateObserver()
        {
            if (SpatialAwarenessSystem == null) { return; }

            if (observer == null &&
                XRGeneralSettings.Instance != null &&
                XRGeneralSettings.Instance.Manager != null &&
                XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                observer = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRMeshSubsystem>();

                if (observer != null)
                {
                    ConfigureObserverVolume();

                    if (StartupBehavior == AutoStartBehavior.AutoStart)
                    {
                        Resume();
                    }
                }
            }
        }

        /// <summary>
        /// Implements proper cleanup of the SurfaceObserver.
        /// </summary>
        protected override void CleanupObserver()
        {
            if (IsRunning)
            {
                Suspend();
            }

            if (observer != null)
            {
                observer.Destroy();
                observer = null;
            }
        }

        /// <summary>
        /// Requests updates from the surface observer.
        /// </summary>
        private void UpdateObserver()
        {
            if (SpatialAwarenessSystem == null || observer == null) { return; }

            // Only update the observer if it is running.
            if (IsRunning && (outstandingMeshObject == null))
            {
                // If we have a mesh to work on...
                if (meshWorkQueue.Count > 0)
                {
                    // We're using a simple first-in-first-out rule for requesting meshes, but a more sophisticated algorithm could prioritize
                    // the queue based on distance to the user or some other metric.
                    RequestMesh(meshWorkQueue.Dequeue());
                }
                // If enough time has passed since the previous observer update...
                else if (Time.time - lastUpdated >= UpdateInterval)
                {
                    // Update the observer orientation if user aligned
                    if (ObserverVolumeType == VolumeType.UserAlignedCube)
                    {
                        ObserverRotation = CameraCache.Main.transform.rotation;
                    }

                    // Update the observer location if it is not stationary
                    if (!IsStationaryObserver)
                    {
                        ObserverOrigin = CameraCache.Main.transform.position;
                    }

                    // The application can update the observer volume at any time, make sure we are using the latest.
                    ConfigureObserverVolume();

                    if (observer.TryGetMeshInfos(meshInfos))
                    {
                        UpdateMeshes(meshInfos);
                    }

                    lastUpdated = Time.time;
                }
            }
        }

        /// <summary>
        /// Internal component to monitor the WorldAnchor's transform, apply the MixedRealityPlayspace transform,
        /// and apply it to its parent.
        /// </summary>
        private class PlayspaceAdapter : MonoBehaviour
        {
            /// <summary>
            /// Compute concatenation of lhs * rhs such that lhs * (rhs * v) = Concat(lhs, rhs) * v
            /// </summary>
            /// <param name="lhs">Second transform to apply</param>
            /// <param name="rhs">First transform to apply</param>
            private static Pose Concatenate(Pose lhs, Pose rhs)
            {
                return rhs.GetTransformedBy(lhs);
            }

            /// <summary>
            /// Compute and set the parent's transform.
            /// </summary>
            private void Update()
            {
                Pose worldFromPlayspace = new Pose(MixedRealityPlayspace.Position, MixedRealityPlayspace.Rotation);
                Pose anchorPose = new Pose(transform.position, transform.rotation);
                Pose parentPose = Concatenate(worldFromPlayspace, anchorPose);
                transform.parent.position = parentPose.position;
                transform.parent.rotation = parentPose.rotation;
            }
        }

        /// <summary>
        /// Issue a request to the Surface Observer to begin baking the mesh.
        /// </summary>
        /// <param name="meshId">ID of the mesh to bake.</param>
        private void RequestMesh(MeshId meshId)
        {
            string meshName = ("SpatialMesh - " + meshId);

            SpatialAwarenessMeshObject newMesh;
            //WorldAnchor worldAnchor;

            if (spareMeshObject == null)
            {
                newMesh = SpatialAwarenessMeshObject.Create(
                    null,
                    MeshPhysicsLayer,
                    meshName,
                    meshId.GetHashCode());

                // The WorldAnchor component places its object where the anchor is in the same space as the camera. 
                // But since the camera is repositioned by the MixedRealityPlayspace's transform, the meshes' transforms
                // should also the WorldAnchor position repositioned by the MixedRealityPlayspace's transform.
                // So rather than put the WorldAnchor on the mesh's GameObject, the WorldAnchor is placed out of the way in the scene,
                // and its transform is concatenated with the Playspace transform to compute the transform on the mesh's object.
                // That adapting the WorldAnchor's transform into playspace is done by the internal PlayspaceAdapter component.
                // The GameObject the WorldAnchor is placed on is unimportant, but it is convenient for cleanup to make it a child
                // of the GameObject whose transform will track it.
                GameObject anchorHolder = new GameObject(meshName + "_anchor");
                anchorHolder.AddComponent<PlayspaceAdapter>(); // replace with required component?
                //worldAnchor = anchorHolder.AddComponent<WorldAnchor>(); // replace with required component and GetComponent()? 
                anchorHolder.transform.SetParent(newMesh.GameObject.transform, false);
            }
            else
            {
                newMesh = spareMeshObject;
                spareMeshObject = null;

                newMesh.GameObject.name = meshName;
                newMesh.Id = meshId.GetHashCode();
                newMesh.GameObject.SetActive(true);

                //// There should be exactly one child on the newMesh.GameObject, and that is the GameObject added above
                //// to hold the WorldAnchor component and adapter.
                //Debug.Assert(newMesh.GameObject.transform.childCount == 1, "Expecting a single child holding the WorldAnchor");
                //worldAnchor = newMesh.GameObject.transform.GetChild(0).gameObject.GetComponent<WorldAnchor>();
            }

            //Debug.Assert(worldAnchor != null);

            observer.GenerateMeshAsync(meshId, newMesh.Filter.mesh, newMesh.Collider, MeshVertexAttributes.Normals, (MeshGenerationResult meshGenerationResult) => MeshGenerationAction(meshGenerationResult));
            outstandingMeshObject = newMesh;
        }

        /// <summary>
        /// Removes the <see cref="SpatialAwarenessMeshObject"/> associated with the specified id.
        /// </summary>
        /// <param name="id">The id of the mesh to be removed.</param>
        protected void RemoveMeshObject(int id)
        {
            SpatialAwarenessMeshObject mesh;

            if (meshes.TryGetValue(id, out mesh))
            {
                // Remove the mesh object from the collection.
                meshes.Remove(id);

                // Reclaim the mesh object for future use.
                ReclaimMeshObject(mesh);

                // Send the mesh removed event
                meshEventData.Initialize(this, id, null);
                SpatialAwarenessSystem?.HandleEvent(meshEventData, OnMeshRemoved);
            }
        }

        /// <summary>
        /// Reclaims the <see cref="SpatialAwarenessMeshObject"/> to allow for later reuse.
        /// </summary>
        protected void ReclaimMeshObject(SpatialAwarenessMeshObject availableMeshObject)
        {
            if (spareMeshObject == null)
            {
                // Cleanup the mesh object.
                // Do not destroy the game object, destroy the meshes.
                SpatialAwarenessMeshObject.Cleanup(availableMeshObject, false);

                availableMeshObject.GameObject.name = "Unused Spatial Mesh";
                availableMeshObject.GameObject.SetActive(false);

                spareMeshObject = availableMeshObject;
            }
            else
            {
                // Cleanup the mesh object.
                // Destroy the game object, destroy the meshes.
                SpatialAwarenessMeshObject.Cleanup(availableMeshObject);
            }
        }

        /// <summary>
        /// Applies the configured observation extents.
        /// </summary>
        private void ConfigureObserverVolume()
        {
            if (SpatialAwarenessSystem == null || observer == null)
            {
                return;
            }

            // Update the observer
            switch (ObserverVolumeType)
            {
                case VolumeType.AxisAlignedCube:
                    observer.SetBoundingVolume(ObserverOrigin, ObservationExtents);
                    break;

                case VolumeType.Sphere:
                    // We use the x value of the extents as the sphere radius
                    observer.SetBoundingVolumeSphere(ObserverOrigin, ObservationExtents.x);
                    break;

                case VolumeType.UserAlignedCube:
                    observer.SetBoundingVolumeOrientedBox(ObserverOrigin, ObservationExtents, ObserverRotation);
                    break;

                default:
                    Debug.LogError($"Unsupported ObserverVolumeType value {ObserverVolumeType}");
                    break;
            }
        }

        /// <summary>
        /// Handles the SurfaceObserver's OnSurfaceChanged event.
        /// </summary>
        /// <param name="id">The identifier assigned to the surface which has changed.</param>
        /// <param name="changeType">The type of change that occurred on the surface.</param>
        /// <param name="bounds">The bounds of the surface.</param>
        /// <param name="updateTime">The date and time at which the change occurred.</param>
        private void UpdateMeshes(List<MeshInfo> meshInfos)
        {
            if (!IsRunning) { return; }

            foreach (MeshInfo meshInfo in meshInfos)
            {
                switch (meshInfo.ChangeState)
                {
                    case MeshChangeState.Added:
                    case MeshChangeState.Updated:
                        meshWorkQueue.Enqueue(meshInfo.MeshId);
                        break;

                    case MeshChangeState.Removed:
                        RemoveMeshObject(meshInfo.MeshId.GetHashCode());
                        break;
                }
            }
        }

        private void MeshGenerationAction(MeshGenerationResult meshGenerationResult)
        {
            if (!IsRunning) { return; }

            if (outstandingMeshObject == null)
            {
                Debug.LogWarning($"MeshGenerationAction called for mesh id {meshGenerationResult.MeshId} while no request was outstanding.");
                return;
            }

            switch (meshGenerationResult.Status)
            {
                case MeshGenerationStatus.InvalidMeshId:
                case MeshGenerationStatus.Canceled:
                case MeshGenerationStatus.UnknownError:
                    outstandingMeshObject = null;
                    break;
                case MeshGenerationStatus.Success:
                    // Since there is only one outstanding mesh object, update the id to match
                    // the one received after baking.
                    SpatialAwarenessMeshObject meshObject = outstandingMeshObject;
                    meshObject.Id = meshGenerationResult.MeshId.GetHashCode();
                    outstandingMeshObject = null;

                    // Apply the appropriate material to the mesh.
                    SpatialAwarenessMeshDisplayOptions displayOption = DisplayOption;
                    if (displayOption != SpatialAwarenessMeshDisplayOptions.None)
                    {
                        meshObject.Renderer.enabled = true;
                        meshObject.Renderer.sharedMaterial = (displayOption == SpatialAwarenessMeshDisplayOptions.Visible) ?
                            VisibleMaterial :
                            OcclusionMaterial;
                    }
                    else
                    {
                        meshObject.Renderer.enabled = false;
                    }

                    // Recalculate the mesh normals if requested.
                    if (RecalculateNormals)
                    {
                        meshObject.Filter.sharedMesh.RecalculateNormals();
                    }

                    // Add / update the mesh to our collection
                    bool sendUpdatedEvent = false;
                    if (meshes.ContainsKey(meshObject.Id))
                    {
                        // Reclaim the old mesh object for future use.
                        ReclaimMeshObject(meshes[meshObject.Id]);
                        meshes.Remove(meshObject.Id);

                        sendUpdatedEvent = true;
                    }
                    meshes.Add(meshObject.Id, meshObject);

                    meshObject.GameObject.transform.parent = (ObservedObjectParent.transform != null) ? ObservedObjectParent.transform : null;

                    meshEventData.Initialize(this, meshObject.Id, meshObject);
                    if (sendUpdatedEvent)
                    {
                        SpatialAwarenessSystem?.HandleEvent(meshEventData, OnMeshUpdated);
                    }
                    else
                    {
                        SpatialAwarenessSystem?.HandleEvent(meshEventData, OnMeshAdded);
                    }
                    break;
            }
        }

        /// <inheritdoc />
        public override void ClearObservations()
        {
            bool wasRunning = false;

            if (IsRunning)
            {
                wasRunning = true;
                Debug.Log("Cannot clear observations while the observer is running. Suspending this observer.");
                Suspend();
            }

            IReadOnlyList<int> observations = new List<int>(Meshes.Keys);
            foreach (int meshId in observations)
            {
                RemoveMeshObject(meshId);
            }

            if (wasRunning)
            {
                Resume();
            }
        }

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}
