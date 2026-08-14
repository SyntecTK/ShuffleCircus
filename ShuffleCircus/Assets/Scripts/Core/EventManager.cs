using System;

public static class EventManager 
{
    //--------------GameBoard-------------
    public static event Action<int, int, bool> OnCardDropped;
    public static event Action OnTurnEnded;
    public static event Action OnBoardChanged;
    public static event Action OnGameOver;
    public static event Action OnHandDrawStarted;
    public static event Action OnDrawnHand;
    public static event Action OnPause;
    public static event Action OnCardStolen;

    //DELETE LATER
    public static event Action TutorialOver;

    //----------Artifacts-----------------
    public static event Action OnAddedArtifact;

    //-------GameBoard Methods------------
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

    public static void HandDrawStarted()
    {
        OnHandDrawStarted?.Invoke();
    }

    public static void DrawnHand()
    {
        OnDrawnHand?.Invoke();
    }

    public static void CardStolen()
    {
        OnCardStolen?.Invoke();
    }

    public static void Pause()
    {
        OnPause?.Invoke();
    }

    public static void TutorialCompleted()
    {
        TutorialOver?.Invoke();
    }

    //--------- Artifact Methods----------

    public static void AddedArtifact()
    {
        OnAddedArtifact?.Invoke();
    }
    

}
