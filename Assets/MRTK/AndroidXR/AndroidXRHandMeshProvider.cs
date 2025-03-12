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

            List<XRMeshSubsystem> meshSubsystems = new List<XRMeshSubsystem>();
            SubsystemManager.GetSubsystems(meshSubsystems);
            foreach (XRMeshSubsystem subsystem in meshSubsystems)
            {
                Debug.Log(subsystem.subsystemDescriptor.id);
            }

            //meshSubsystem = meshSubsystems[0];

            mesh = new Mesh();
            neutralPoseMesh = new Mesh();
        }

        private readonly XRMeshSubsystem meshSubsystem;
        private readonly Utilities.Handedness handedness;
        private readonly Mesh mesh;
        private readonly Mesh neutralPoseMesh;

        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector3> normals = new List<Vector3>();
        private readonly List<int> triangles = new List<int>();

        private Vector2[] handMeshUVs = null;

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
                    if (handMeshUVs != null)
                    {
                        // Notify that hand mesh has been updated (cleared)
                        CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, new HandMeshInfo());
                        handMeshUVs = null;
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
                            null, MeshVertexAttributes.Normals, result => { });

                        mesh.GetVertices(vertices);
                        mesh.GetNormals(normals);
                        mesh.GetTriangles(triangles, 0);

                        HandMeshInfo handMeshInfo = new HandMeshInfo
                        {
                            vertices = vertices.ToArray(),
                            normals = normals.ToArray(),
                            triangles = triangles.ToArray(),
                            uvs = handMeshUVs,
                            position = MixedRealityPlayspace.Position,
                            rotation = MixedRealityPlayspace.Rotation
                        };

                        CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, handMeshInfo);
                    }
                }                    
            }
        }

        private Vector2[] InitializeUVs(Vector3[] neutralPoseVertices)
        {
            if (neutralPoseVertices.Length == 0)
            {
                Debug.LogError("Loaded 0 vertices for neutralPoseVertices");
                return System.Array.Empty<Vector2>();
            }

            float minY = neutralPoseVertices[0].y;
            float maxY = minY;

            for (int ix = 1; ix < neutralPoseVertices.Length; ix++)
            {
                Vector3 p = neutralPoseVertices[ix];

                if (p.y < minY)
                {
                    minY = p.y;
                }
                else if (p.y > maxY)
                {
                    maxY = p.y;
                }
            }

            float scale = 1.0f / (maxY - minY);

            Vector2[] uvs = new Vector2[neutralPoseVertices.Length];

            for (int ix = 0; ix < neutralPoseVertices.Length; ix++)
            {
                Vector3 p = neutralPoseVertices[ix];

                uvs[ix] = new Vector2(p.x * scale + 0.5f, (p.y - minY) * scale);
            }

            return uvs;
        }
    }
}
