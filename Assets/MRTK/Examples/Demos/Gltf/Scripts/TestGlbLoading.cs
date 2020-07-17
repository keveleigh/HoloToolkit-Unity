// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.Gltf
{
    /// <summary>
    /// glb loading test script that attempts to download the asset from a local or external resource via web request.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/TestGlbLoading")]
    public class TestGlbLoading : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("This can be a local or external resource uri.")]
        private string uri = string.Empty;

        [SerializeField]
        [Tooltip("An optional material to use on the loaded model.")]
        private Material modelMaterial = null;

        private async void Start()
        {
            Utilities.Response response = new Utilities.Response();

            try
            {
                response = await Utilities.Rest.GetAsync(uri, readResponseData: true);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            if (!response.Successful)
            {
                Debug.LogError($"Failed to get glb model from {uri}");
                return;
            }

            var gltfObject = GltfUtility.GetGltfObjectFromGlb(response.ResponseData);

            try
            {
                await gltfObject.ConstructAsync(modelMaterial);
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
                return;
            }

            if (gltfObject != null)
            {
                Debug.Log("Import successful");
            }
        }
    }
}