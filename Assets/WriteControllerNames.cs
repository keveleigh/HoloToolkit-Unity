using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class WriteControllerNames : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro text = null;

    private void Update()
    {
        InputDevice left = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice right = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        text.text = $"{left.name} | {left.manufacturer} | {left.serialNumber}\n";
        text.text += $"{right.name} | {right.manufacturer} | {right.serialNumber}\n";
    }
}
