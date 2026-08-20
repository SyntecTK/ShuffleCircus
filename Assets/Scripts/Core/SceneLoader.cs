using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private bool isLoading = false;

    public void LoadScene(string sceneName)
    {
        if (isLoading) return;

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    public void LoadSceneAdditive(string sceneName)
    {
        if (isLoading) return;

        StartCoroutine(LoadSceneAdditiveRoutine(sceneName));
    }

    public void UnloadAdditiveScene(string sceneName)
    {
        if (isLoading) return;

        StartCoroutine(UnloadAdditiveSceneRoutine(sceneName));
    }


    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;
        yield return SceneManager.LoadSceneAsync(sceneName);
        isLoading = false;
    }

    private IEnumerator LoadSceneAdditiveRoutine(string sceneName)
    {
        isLoading = true;
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        isLoading = false;
    }

    private IEnumerator UnloadAdditiveSceneRoutine(string sceneName)
    {
        isLoading = true;
        yield return SceneManager.UnloadSceneAsync(sceneName);
        isLoading = false;
    }
}
