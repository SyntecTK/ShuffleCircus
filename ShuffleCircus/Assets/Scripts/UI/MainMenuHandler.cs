using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private InputField playerNameInput;

    public void StartGame()
    {
        SceneManager.LoadScene("Map");
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
