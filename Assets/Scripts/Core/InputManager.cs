using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private bool canPassTurn;
    public bool CanPassTurn => canPassTurn;

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
        if(GameManager.Instance.State.GameMode == GameMode.Singleplayer)
        {
            if (GameManager.Instance != null && 
                (GameManager.Instance.IsGameOver || 
                !GameManager.Instance.IsPlayerTurn))
            {
                return;
            }
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            {
                return;
            }
        }

        if(canPassTurn)
        {
            canPassTurn = false;
            GameManager.Instance.EndTurn();
            EventManager.BoardChanged();
        }
    }

    private void OnExit()
    {
        EventManager.Pause();
    }
}
