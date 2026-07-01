using System.Data.SqlTypes;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private const int MaxCardsPerTurn = 4;

    [SerializeField] private GameState gameState;
    [SerializeField] private bool isPlayerTurn = true;
    [SerializeField] private int cardsPlayedThisTurn;
    [SerializeField] private bool isGameOver;

    public void EndTurn()
    {
        if (isGameOver)
        {
            return;
        }

        isPlayerTurn = !isPlayerTurn;
        cardsPlayedThisTurn = 0;
        EventManager.TurnEnded();
    }

    public GameState State => gameState;
    public bool IsPlayerTurn => isPlayerTurn;
    public bool IsGameOver => isGameOver;
    public int CardsPlayedThisTurn => cardsPlayedThisTurn;
    public int CardsRemainingThisTurn => MaxCardsPerTurn - cardsPlayedThisTurn;

    public bool CanPlayCardThisTurn()
    {
        if (isGameOver)
        {
            return false;
        }

        return cardsPlayedThisTurn < MaxCardsPerTurn;
    }

    public bool TryRegisterPlayedCard()
    {
        if (isGameOver)
        {
            return false;
        }

        if (!CanPlayCardThisTurn())
        {
            return false;
        }

        cardsPlayedThisTurn++;
        return true;
    }

    public void SetPlayerTurn(bool isPlayerTurn)
    {
        this.isPlayerTurn = isPlayerTurn;
        cardsPlayedThisTurn = 0;
        isGameOver = false;
    }

    public void SetOpponentTurn(bool isOpponentTurn)
    {
        isPlayerTurn = !isOpponentTurn;
        cardsPlayedThisTurn = 0;
        isGameOver = false;
    }

    public void SetGameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        EventManager.GameOver();
    }
}
