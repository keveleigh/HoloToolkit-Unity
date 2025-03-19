// Copyright (c) Microsoft Corporation.
// Copyright (c) Qualcomm.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK.AndroidXR
{
    public class AndroidXRHandMeshProvider
    {
        /// <summary>
        /// The user's left hand.
        /// </summary>
        public static AndroidXRHandMeshProvider Left { get; } = new AndroidXRHandMeshProvider(Utilities.Handedness.Left);

        /// <summary>
        /// The user's right hand.
        /// </summary>
        public static AndroidXRHandMeshProvider Right { get; } = new AndroidXRHandMeshProvider(Utilities.Handedness.Right);

        private AndroidXRHandMeshProvider(Utilities.Handedness handedness)
        {
            this.handedness = handedness;
            mesh = new Mesh();

            List<XRMeshSubsystem> meshSubsystems = new List<XRMeshSubsystem>();
            SubsystemManager.GetSubsystems(meshSubsystems);
            foreach (XRMeshSubsystem subsystem in meshSubsystems)
            {
                if (subsystem.subsystemDescriptor.id == "AndroidXRHandMeshProvider")
                {
                    meshSubsystem = subsystem;
                    break;
                }
            }
        }

        private readonly XRMeshSubsystem meshSubsystem = null;
        private readonly Utilities.Handedness handedness;
        private readonly Mesh mesh;

        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector3> normals = new List<Vector3>();
        private readonly List<int> triangles = new List<int>();

        private IMixedRealityInputSource inputSource = null;

        /// <summary>
        /// Sets the <see cref="IMixedRealityInputSource"/> that represents the current hand for this mesh.
        /// </summary>
        /// <param name="inputSource">Implementation of the hand input source.</param>
        public void SetInputSource(IMixedRealityInputSource inputSource) => this.inputSource = inputSource;

        private static readonly ProfilerMarker UpdateHandMeshPerfMarker = new ProfilerMarker($"[MRTK] {nameof(AndroidXRHandMeshProvider)}.UpdateHandMesh");

        /// <summary>
        /// Updates the hand mesh based on the current state of the hand.
        /// </summary>
        public void UpdateHandMesh()
        {
            using (UpdateHandMeshPerfMarker.Auto())
            {
                MixedRealityInputSystemProfile inputSystemProfile = CoreServices.InputSystem?.InputSystemProfile;
                MixedRealityHandTrackingProfile handTrackingProfile = inputSystemProfile != null ? inputSystemProfile.HandTrackingProfile : null;

                if (handTrackingProfile == null || !handTrackingProfile.EnableHandMeshVisualization || meshSubsystem == null)
                {
                    // If hand mesh visualization is disabled make sure to clean up if we've already initialized
                    if (triangles.Count > 0)
                    {
                        // Notify that hand mesh has been updated (cleared)
                        CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, new HandMeshInfo());
                        triangles.Clear();
                    }
                    return;
                }

                List<MeshInfo> meshInfos = new List<MeshInfo>();
                if (meshSubsystem.TryGetMeshInfos(meshInfos))
                {
                    int handMeshIndex = handedness == Utilities.Handedness.Left ? 0 : 1;

                    if (meshInfos[handMeshIndex].ChangeState == MeshChangeState.Added
                        || meshInfos[handMeshIndex].ChangeState == MeshChangeState.Updated)
                    {
                        meshSubsystem.GenerateMeshAsync(meshInfos[handMeshIndex].MeshId, mesh,
                            null, MeshVertexAttributes.Normals, result =>
                            {
                                if (result.Status != MeshGenerationStatus.Success)
                                {
                                    Debug.LogWarning($"Hand mesh not properly generated: {result.Status}");
                                    return;
                                }

                                result.Mesh.GetVertices(vertices);
                                result.Mesh.GetNormals(normals);
                                result.Mesh.GetTriangles(triangles, 0);

                                HandMeshInfo handMeshInfo = new HandMeshInfo
                                {
                                    vertices = vertices.ToArray(),
                                    normals = normals.ToArray(),
                                    triangles = triangles.ToArray(),
                                    position = MixedRealityPlayspace.Position,
                                    rotation = MixedRealityPlayspace.Rotation
                                };

                                CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, handMeshInfo);
                            });
                    }
                }                    
            }
        }
    }
}
