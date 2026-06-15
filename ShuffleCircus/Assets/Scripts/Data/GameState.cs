using UnityEngine;

public enum GameMode
{
    Singleplayer,
    LocalMultiplayer
}

[CreateAssetMenu]
public class GameState : ScriptableObject
{
    [SerializeField] private GameMode gameMode;
    [SerializeField] private string player1Name;
    [SerializeField] private string player2Name;

    public GameMode GameMode => gameMode;
    public string Player1Name => player1Name;
    public string Player2Name => player2Name;

    public void SetGameMode(GameMode mode)
    {
        gameMode = mode;
    }

    public void SetPlayerNames(string player1, string player2)
    {
        player1Name = player1;
        player2Name = player2;
    }

    public void ResetSession()
    {
        gameMode = GameMode.Singleplayer;
        player1Name = string.Empty;
        player2Name = string.Empty;
    }

}