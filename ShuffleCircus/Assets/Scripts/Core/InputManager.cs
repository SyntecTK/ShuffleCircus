using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private bool canPassTurn;

    private void OnEnable()
    {
        EventManager.OnHandDrawStarted += OnHandDrawStarted;
        EventManager.OnDrawnHand += OnDrawnHand;
    }

    private void OnDisable()
    {
        EventManager.OnHandDrawStarted -= OnHandDrawStarted;
        EventManager.OnDrawnHand -= OnDrawnHand;
    }

    private void OnHandDrawStarted()
    {
        canPassTurn = false;
    }

    private void OnDrawnHand()
    {
        canPassTurn = true;
    }

    private void OnInteract()
    {
        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver 
            || !GameManager.Instance.IsPlayerTurn))
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
