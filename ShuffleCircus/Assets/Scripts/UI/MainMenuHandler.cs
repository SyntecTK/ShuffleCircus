using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public void StartGameSolo()
    {
        GameManager.Instance.State.SetGameMode(GameMode.Singleplayer);
        ArtifactManager.Instance.ClearActiveArtifacts();
        DeckManager.Instance.ResetDecks();
        GameManager.Instance.ResetBattleCounter();
        GameManager.Instance.State.SetAIDifficultyLevel(1);
        SceneManager.LoadScene("Intro");
    }

    public void StartGameMultiplayer()
    {
        GameManager.Instance.State.SetGameMode(GameMode.Multiplayer);
        SceneManager.LoadScene("MultiplayerIntro");
    }

    public void OpenSettings()
    {
        Debug.Log("TODO");
    }

    public void OpenDiscord()
    {
        Application.OpenURL("https://discord.gg/WMEX29QcE");
    }

    public void OpenFeedbackFormular()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSc7xMaxTgi1FGNSY-f8XovB5rBEURyU30VkiExLBqsuF_Qypg/viewform?usp=publish-editor");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
