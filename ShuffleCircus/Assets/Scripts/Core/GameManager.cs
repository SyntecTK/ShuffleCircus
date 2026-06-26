using System.Data.SqlTypes;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private const int MaxCardsPerTurn = 4;

    [SerializeField] private GameState gameState;
    [SerializeField] private bool isPlayerTurn = true;
    [SerializeField] private int cardsPlayedThisTurn;

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        cardsPlayedThisTurn = 0;
        EventManager.TurnEnded();
    }

    public GameState State => gameState;
    public bool IsPlayerTurn => isPlayerTurn;
    public int CardsPlayedThisTurn => cardsPlayedThisTurn;
    public int CardsRemainingThisTurn => MaxCardsPerTurn - cardsPlayedThisTurn;

    public bool CanPlayCardThisTurn()
    {
        return cardsPlayedThisTurn < MaxCardsPerTurn;
    }

    public bool TryRegisterPlayedCard()
    {
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
    }

    public void SetOpponentTurn(bool isOpponentTurn)
    {
        isPlayerTurn = !isOpponentTurn;
        cardsPlayedThisTurn = 0;
    }
}
