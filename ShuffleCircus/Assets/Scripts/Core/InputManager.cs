using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private void OnEnable()
    {
        EventManager.OnDrawnHand += OnDrawnHand;
    }

    private void OnDisable()
    {
        EventManager.OnDrawnHand -= OnDrawnHand;
    }

    private bool canPassTurn = true;

    private void OnDrawnHand()
    {
        canPassTurn = true; 
    }

    private void OnInteract()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if(canPassTurn)
        {
            canPassTurn = false;
            GameManager.Instance.EndTurn();
            EventManager.BoardChanged();
        }
    }
}
