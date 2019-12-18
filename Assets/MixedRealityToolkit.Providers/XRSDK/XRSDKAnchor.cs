// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

public class XRSDKAnchor : MonoBehaviour
{
    private XRReferencePointSubsystem referencePointSubsystem;
    private XRReferencePoint referencePoint;
    private Material material;

    private void Awake()
    {
        if (XRGeneralSettings.Instance != null &&
            XRGeneralSettings.Instance.Manager != null &&
            XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            referencePointSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRReferencePointSubsystem>();

            if (referencePointSubsystem != null)
            {
                if (referencePointSubsystem.TryAddReferencePoint(new Pose(transform.position, transform.rotation), out referencePoint))
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
        if (referencePointSubsystem == null)
        {
            return;
        }

        TrackableChanges<XRReferencePoint> currentReferencePoints = referencePointSubsystem.GetChanges(Allocator.Temp);

        foreach (XRReferencePoint rp in currentReferencePoints.added)
        {
            if (rp.trackableId == referencePoint.trackableId)
            {
                material.color = Color.blue;
                transform.position = rp.pose.position;
                transform.rotation = rp.pose.rotation;
                return;
            }
        }

        foreach (XRReferencePoint rp in currentReferencePoints.updated)
        {
            if (rp.trackableId == referencePoint.trackableId)
            {
                material.color = Color.green;
                transform.position = rp.pose.position;
                transform.rotation = rp.pose.rotation;
                return;
            }
        }

        foreach (TrackableId trackableId in currentReferencePoints.removed)
        {
            if (trackableId == referencePoint.trackableId)
            {
                material.color = Color.red;
                return;
            }
        }
    }

    private void OnDestroy()
    {
        if (referencePointSubsystem != null)
        {
            if (referencePointSubsystem.TryRemoveReferencePoint(referencePoint.trackableId))
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
