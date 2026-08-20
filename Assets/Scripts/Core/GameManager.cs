using System.Data.SqlTypes;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private const int MaxCardsPerTurn = 4;
    private const int MaxBattles = 3;
    private int currentBattleCount = 0;
    public int CurrentBattleCount => currentBattleCount;
    public int MaxBattlesAllowed => MaxBattles;

    [SerializeField] private GameState gameState;
    [SerializeField] private bool isPlayerTurn = true;
    [SerializeField] private int cardsPlayedThisTurn;
    [SerializeField] private bool isGameOver;

    public GameState State => gameState;
    public bool IsMultiplayer => State.GameMode == GameMode.Multiplayer;

    public bool IsPlayerTurn => isPlayerTurn;
    public bool IsGameOver => isGameOver;

    public int CardsPlayedThisTurn => cardsPlayedThisTurn;
    public int MaxCardsAllowedPerTurn => MaxCardsPerTurn;
    public int CardsRemainingThisTurn => MaxCardsPerTurn - cardsPlayedThisTurn;

    private bool playerIsWinner;

    //---------------------------Tent Progress----------------------------------
    public int SelectedEventFieldID {get; set;} = -1;
    private const int progressSteps = 4;
    private int[] progresses = new int[progressSteps];

    public void IncreaseTentProgress(int eventID)
    {
        if (progresses == null || eventID < 0 || eventID >= progresses.Length)
        {
            Debug.LogWarning("Invalid event ID or progresses array not initialized.");
            return;
        }

        progresses[eventID]++;
    }

    public int GetTentProgress(int eventID)
    {
        if (progresses == null || eventID < 0 || eventID >= progresses.Length)
        {
            Debug.LogWarning("Invalid event ID or progresses array not initialized.");
            return 0;
        }

        return progresses[eventID];
    }

    

    private void Start()
    {
        State.ResetSession();
    }

    public void ResetBattleCounter()
    {
        currentBattleCount = 0;
        PlayerPrefs.SetInt("CurrentBattleCount", currentBattleCount);
    }

    public void IncreaseBattleCounter()
    {
        if (currentBattleCount < MaxBattles)
        {
            currentBattleCount++;
        }
    }

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
