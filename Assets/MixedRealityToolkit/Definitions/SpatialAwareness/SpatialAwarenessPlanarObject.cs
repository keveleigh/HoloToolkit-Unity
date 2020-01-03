// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public partial class SpatialAwarenessPlanarObject : BaseSpatialAwarenessObject
    {
        /// <summary>
        /// The BoxCollider associated with this plane's GameObject.
        /// </summary>
        public BoxCollider Collider { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpatialAwarenessPlanarObject(Vector3 size, int layer, string name, int planeId) : base()
        {
            Id = planeId;
            GameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject.layer = layer;
            GameObject.transform.localScale = size;

            Filter = newMesh.GameObject.GetComponent<MeshFilter>();
            Renderer = newMesh.GameObject.GetComponent<MeshRenderer>();
            Collider = newMesh.GameObject.GetComponent<BoxCollider>();
        }

        /// <summary>
        /// Creates a <see cref="SpatialAwarenessPlanarObject"/>.
        /// </summary>
        /// <returns>
        /// SpatialAwarenessPlanarObject containing the fields that describe the plane.
        /// </returns>
        public static SpatialAwarenessPlanarObject CreateSpatialObject(Vector3 size, int layer, string name, int planeId)
        {
            return new SpatialAwarenessPlanarObject(size, layer, name, planeId);
        }
    }
}