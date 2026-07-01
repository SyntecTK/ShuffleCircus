using System;

public static class EventManager 
{
    public static event Action<int, int, bool> OnCardDropped;
    public static event Action OnTurnEnded;
    public static event Action OnBoardChanged;
    public static event Action OnGameOver;
    public static event Action OnDrawnHand;

    public static void CardDropped(int rowIndex, int columnIndex, bool isPlayerSlot)
    {
        OnCardDropped?.Invoke(rowIndex, columnIndex, isPlayerSlot);
    }

    public static void TurnEnded()
    {
        OnTurnEnded?.Invoke();
    }

    public static void BoardChanged()
    {
        OnBoardChanged?.Invoke();
    }

    public static void GameOver()
    {
        OnGameOver?.Invoke();
    }

    public static void DrawnHand()
    {
        OnDrawnHand?.Invoke();
    }
}
