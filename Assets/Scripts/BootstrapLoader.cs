using UnityEngine;
using UnityEngine.SceneManagement;

public static class BootstrapLoader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void LoadBootstrap()
    {
        if (Object.FindFirstObjectByType<GameManager>() != null)
            return;

        SceneManager.LoadScene("Bootstrapper");
    }
}
