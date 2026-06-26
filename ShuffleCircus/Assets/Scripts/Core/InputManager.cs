using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private void OnInteract()
    {
        Debug.Log("Interact action triggered.");
        GameManager.Instance.EndTurn();
        EventManager.BoardChanged();
    }
}
