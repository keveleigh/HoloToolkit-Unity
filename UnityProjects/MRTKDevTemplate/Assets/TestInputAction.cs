using UnityEngine;
using UnityEngine.InputSystem;

public class TestInputAction : MonoBehaviour
{
    [SerializeField]
    private InputAction action;

    private Material material;

    private void OnEnable()
    {
        if (action == null)
        {
            return;
        }

        action.started += Action_started;
        action.canceled += Action_canceled;
        action.Enable();

        material = GetComponent<Renderer>().material;
    }

    private void Action_started(InputAction.CallbackContext obj)
    {
        material.color = Color.blue;
    }

    private void Action_canceled(InputAction.CallbackContext obj)
    {
        material.color = Color.green;
    }

    private void OnDisable()
    {
        action.Disable();
        action.canceled -= Action_canceled;
        action.started -= Action_started;
    }
}
