// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

public class XRSDKAnchor : MonoBehaviour
{
    private XRAnchorSubsystem anchorSubsystem;
    private XRAnchor currentAnchor;
    private Material material;

    private void Awake()
    {
        if (XRGeneralSettings.Instance != null &&
            XRGeneralSettings.Instance.Manager != null &&
            XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            anchorSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRAnchorSubsystem>();

            if (anchorSubsystem != null)
            {
                if (anchorSubsystem.TryAddAnchor(new Pose(transform.position, transform.rotation), out currentAnchor))
                {
                    Debug.Log("Successfully created anchor.");
                }
                else
                {
                    Debug.Log("Anchor creation failed.");
                }
            }
        }

        material = GetComponent<Renderer>().sharedMaterial;
    }

    private void Update()
    {
        if (anchorSubsystem == null)
        {
            return;
        }

        TrackableChanges<XRAnchor> currentAnchors = anchorSubsystem.GetChanges(Allocator.Temp);

        foreach (XRAnchor anchor in currentAnchors.added)
        {
            if (anchor.trackableId == currentAnchor.trackableId)
            {
                material.color = Color.blue;
                transform.position = anchor.pose.position;
                transform.rotation = anchor.pose.rotation;
                return;
            }
        }

        foreach (XRAnchor rp in currentAnchors.updated)
        {
            if (rp.trackableId == currentAnchor.trackableId)
            {
                material.color = Color.green;
                transform.position = rp.pose.position;
                transform.rotation = rp.pose.rotation;
                return;
            }
        }

        foreach (TrackableId trackableId in currentAnchors.removed)
        {
            if (trackableId == currentAnchor.trackableId)
            {
                material.color = Color.red;
                return;
            }
        }
    }

    private void OnDestroy()
    {
        if (anchorSubsystem != null)
        {
            if (anchorSubsystem.TryRemoveAnchor(currentAnchor.trackableId))
            {
                Debug.Log("Successfully removed anchor.");
            }
            else
            {
                Debug.Log("Anchor removal failed.");
            }
        }
    }
}
