using UnityEngine;
using UnityEngine.UI;

public class Eventfield : MonoBehaviour
{
    [SerializeField] private bool isStartField;
    [SerializeField] private string boardSceneName = "GameBoard";

    private readonly Color selectableColor = new Color(0.5f, 0.8f, 1f, 1f);
    private Image image;
    private GameObject eventSystem;
    private GraphicRaycaster mapRaycaster;
    private Camera mainCam;

    private bool isSelectable;
    private bool isFirstLoad = true;

    private void Start()
    {
        eventSystem = GameObject.Find("EventSystem");
        mapRaycaster = GetComponentInParent<GraphicRaycaster>();

        if (eventSystem != null && !eventSystem.activeInHierarchy)
        {
            EnableEventSystem();
        }

        image = GetComponent<Image>();

        if (isStartField && isFirstLoad)
        {
            image.color = selectableColor;
            isSelectable = true;
            isFirstLoad = false;
        }
        
        mainCam = Camera.main;
    }

    public void OnLevelSelected()
    {
        if (!isSelectable)
        {
            return;
        }

        DisableEventSystem();
        DisableMapRaycasts();

        SceneLoader.Instance.LoadSceneAdditive(boardSceneName);
        image.color = Color.deepPink;
    }

    private void EnableEventSystem()
    {
        if (eventSystem != null)
        {
            eventSystem.SetActive(true);
        }
    }

    private void DisableEventSystem()
    {
        if (eventSystem != null)
        {
            eventSystem.SetActive(false);
        }
    }

    private void DisableMapRaycasts()
    {
        if (mapRaycaster != null)
        {
            mapRaycaster.enabled = false;
        }
    }
}
