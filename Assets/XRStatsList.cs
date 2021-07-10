using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRStatsList : MonoBehaviour
{
    private void Update()
    {
        if (XRStats.TryGetDroppedFrameCount(out int droppedFrameCount))
        {
            Debug.Log($"Dropped frames: {droppedFrameCount}");
        }

        if (XRStats.TryGetFramePresentCount(out int framePresentCount))
        {
            Debug.Log($"Frame present count: {framePresentCount}");
        }

        if (XRStats.TryGetGPUTimeLastFrame(out float gpuTimeLastFrame))
        {
            Debug.Log($"GPU time last frame: {gpuTimeLastFrame}");
        }
    }
}
