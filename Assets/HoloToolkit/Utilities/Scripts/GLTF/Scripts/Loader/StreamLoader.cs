using System;
using System.Collections;
using System.IO;

namespace UnityGLTF.Loader
{
    public class StreamLoader : ILoader
    {
        public Stream LoadedStream { get; private set; }

        public StreamLoader(Stream loadedStream)
        {
            LoadedStream = loadedStream;
        }

        public IEnumerator LoadStream(string gltfFilePath)
        {
            if (LoadedStream == null)
            {
                throw new InvalidOperationException("LoadedStream is null.");
            }

            yield return null;
        }
    }
}
