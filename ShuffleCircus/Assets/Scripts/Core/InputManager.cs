using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private void OnInteract()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        Debug.Log("Interact action triggered.");
        GameManager.Instance.EndTurn();
        EventManager.BoardChanged();
    }
}
