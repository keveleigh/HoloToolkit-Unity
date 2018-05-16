using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityGLTF.Loader;

namespace UnityGLTF
{
    /// <summary>
    /// Component to load a GLTF scene with
    /// </summary>
    public class GLTFComponent : MonoBehaviour
    {
        public string GLTFUri = null;
        public bool Multithreaded = true;
        public bool UseStream = false;

        public Stream GLTFStream { get; set; }

        [SerializeField]
        private bool loadOnStart = true;
        public bool LoadOnStart
        {
            get { return loadOnStart; }
            set { loadOnStart = value; }
        }

        public int MaximumLod = 300;
        public GLTFSceneImporter.ColliderType Collider = GLTFSceneImporter.ColliderType.None;

        [SerializeField]
        private Shader shaderOverride = null;
        public Shader ShaderOverride
        {
            get { return shaderOverride; }
            set { shaderOverride = value; }
        }

        IEnumerator Start()
        {
            if (loadOnStart)
            {
                yield return Load();
            }
        }

        public IEnumerator Load()
        {
            GLTFSceneImporter sceneImporter = null;

            if (UseStream)
            {
                if (GLTFStream == null)
                {
                    // Path.Combine treats paths that start with the separator character
                    // as absolute paths, ignoring the first path passed in. This removes
                    // that character to properly handle a filename written with it.
                    GLTFUri = GLTFUri.TrimStart(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
                    string fullPath = Path.Combine(Application.streamingAssetsPath, GLTFUri);
                    string directoryPath = URIHelper.GetDirectoryName(fullPath);
                    sceneImporter = new GLTFSceneImporter(
                        Path.GetFileName(GLTFUri),
                        new FileLoader(directoryPath)
                        );
                }
                else
                {
                    sceneImporter = new GLTFSceneImporter(
                        null,
                        new StreamLoader(GLTFStream)
                        );
                }
            }
            else
            {
                string directoryPath = URIHelper.GetDirectoryName(GLTFUri);
                sceneImporter = new GLTFSceneImporter(
                    URIHelper.GetFileFromUri(new Uri(GLTFUri)),
                    new WebRequestLoader(directoryPath)
                    );
            }

            sceneImporter.SceneParent = gameObject.transform;
            sceneImporter.Collider = Collider;
            sceneImporter.MaximumLod = MaximumLod;
            sceneImporter.CustomShaderName = shaderOverride ? shaderOverride.name : null;

            yield return sceneImporter.LoadScene(-1, Multithreaded);

            // Override the shaders on all materials if a shader is provided
            if (shaderOverride != null)
            {
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    renderer.sharedMaterial.shader = shaderOverride;
                }
            }
        }
    }
}
