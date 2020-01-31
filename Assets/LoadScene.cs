using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour, IMixedRealityInputHandler
{
    [SerializeField]
    private MixedRealityInputAction action = MixedRealityInputAction.None;

    [SerializeField]
    private string newSceneName = string.Empty;

    void IMixedRealityInputHandler.OnInputDown(InputEventData eventData) { }

    void IMixedRealityInputHandler.OnInputUp(InputEventData eventData)
    {
        if (action == eventData.MixedRealityInputAction && !string.IsNullOrWhiteSpace(newSceneName))
        {
            SceneManager.LoadScene(newSceneName);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(MixedRealityToolkit.Instance);
        DontDestroyOnLoad(MixedRealityPlayspace.Transform);
        DontDestroyOnLoad(CoreServices.InputSystem?.FocusProvider?.UIRaycastCamera);

        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler>(this);
    }

    private void OnDestroy()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler>(this);
    }
}
