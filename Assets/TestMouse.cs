using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMouse : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"{UnityEngine.InputSystem.Mouse.current.leftButton.isPressed} | {UnityEngine.InputSystem.Mouse.current.rightButton.isPressed} | {UnityEngine.InputSystem.Mouse.current.middleButton.isPressed} | {UnityEngine.InputSystem.Mouse.current.forwardButton.isPressed} | {UnityEngine.InputSystem.Mouse.current.backButton.isPressed}");
        Debug.Log($"{UnityEngine.InputSystem.Mouse.current.leftButton.isPressed} | {UnityEngine.InputSystem.Mouse.current.rightButton.isPressed} | {UnityEngine.InputSystem.Mouse.current.middleButton.isPressed} | {UnityEngine.InputSystem.Mouse.current.forwardButton.isPressed} | {UnityEngine.InputSystem.Mouse.current.backButton.isPressed}");
    }
}
