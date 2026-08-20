using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string firstScene = "MainMenu";
    void Start()
    {
        SceneLoader.Instance.LoadScene(firstScene);
    }
}
