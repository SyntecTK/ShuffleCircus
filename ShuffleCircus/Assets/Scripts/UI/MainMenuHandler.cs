using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private InputField playerNameInput;

    public void StartGameSolo()
    {
        GameManager.Instance.State.SetGameMode(GameMode.Singleplayer);
        SceneManager.LoadScene("GameBoard");
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

    public void ExitGame()
    {
        Application.Quit();
    }
}
