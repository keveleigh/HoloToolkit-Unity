using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PhoneControllerAdapter : IPoseSource
{
    [SerializeField]
    private InputActionProperty trackingStateActionProperty;

    [SerializeField]
    private InputActionProperty positionActionProperty;

    [SerializeField]
    private InputActionProperty rotationActionProperty;

    private static readonly Vector3 PhoneOffset = new Vector3(0.0f, -0.5f, 0.0f);

    /// <summary>
    /// Tries to get the pose in worldspace composed of the provided input action properties when the position and rotation are tracked
    /// </summary>
    bool IPoseSource.TryGetPose(out Pose pose)
    {
        InputAction trackingStateAction = trackingStateActionProperty.action;
        InputAction positionAction = positionActionProperty.action;
        InputAction rotationAction = rotationActionProperty.action;

        InputTrackingState inputTrackingState = (InputTrackingState)trackingStateAction.ReadValue<int>();

        if (trackingStateAction.HasAnyControls()
            && positionAction.HasAnyControls()
            && rotationAction.HasAnyControls()
            && (inputTrackingState & (InputTrackingState.Position | InputTrackingState.Rotation)) != 0)
        {
            Vector3 position = positionAction.ReadValue<Vector3>();
            if (position == Vector3.zero && (inputTrackingState & InputTrackingState.Position) != 0)
            {
                position = PhoneOffset;
            }

            // Transform the pose into worldspace, as input actions are returned
            // in floor-offset-relative coordinates.
            pose = PlayspaceUtilities.TransformPose(
                new Pose(
                    position,
                    rotationAction.ReadValue<Quaternion>()));
            return true;
        }
        else
        {
            pose = Pose.identity;
            return false;
        }
    }
}
