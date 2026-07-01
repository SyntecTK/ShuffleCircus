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
    [SerializeField] private int aiDifficultyLevel = 3;

    public GameMode GameMode => gameMode;
    public string Player1Name => player1Name;
    public string Player2Name => player2Name;
    public int AIDifficultyLevel => aiDifficultyLevel;

    public void SetGameMode(GameMode mode)
    {
        gameMode = mode;
    }

    public void SetPlayerNames(string player1, string player2)
    {
        player1Name = player1;
        player2Name = player2;
    }

    public void SetAIDifficultyLevel(int difficultyLevel)
    {
        aiDifficultyLevel = Mathf.Clamp(difficultyLevel, 1, 3);
    }

    public int GetAIDifficultyLevel()
    {
        return aiDifficultyLevel;
    }

    public void IncreaseAIDifficultyLevel()
    {
        if (aiDifficultyLevel < 3)
        {
            aiDifficultyLevel++;
        }
    }

    public void DecreaseAIDifficultyLevel()
    {
        if (aiDifficultyLevel > 1)
        {
            aiDifficultyLevel--;
        }
    }

    public void ResetSession()
    {
        gameMode = GameMode.Singleplayer;
        player1Name = string.Empty;
        player2Name = string.Empty;
        aiDifficultyLevel = 1;
    }

}