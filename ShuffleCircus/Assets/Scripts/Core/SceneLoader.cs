using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private bool isLoading = false;

    public void LoadScene(string sceneName)
    {
        if (isLoading) return;

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;
        yield return null; // Wait a frame to ensure any previous operations are completed
        isLoading = false;
    }
}
