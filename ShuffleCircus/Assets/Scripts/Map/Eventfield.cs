using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Eventfield : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject eventSystem;
    private GraphicRaycaster mapRaycaster;
    private GameObject tentProgressUI;
    private Image[] tentProgressSteps;


    private void Start()
    {
        eventSystem = GameObject.Find("EventSystem");
        mapRaycaster = GetComponentInParent<GraphicRaycaster>();
        tentProgressUI = transform.GetChild(0).gameObject;
        tentProgressSteps = tentProgressUI.GetComponentsInChildren<Image>();

        if (eventSystem != null && !eventSystem.activeInHierarchy)
        {
            EnableEventSystem();
        }
    }

    public void OnLevelSelected()
    {
        DisableEventSystem();
        DisableMapRaycasts();

        SceneLoader.Instance.LoadSceneAdditive("GameBoard");
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

    private void ShowTentProgress()
    {
        tentProgressUI.SetActive(true);
    }

    private void HideTentProgress()
    {
        tentProgressUI.SetActive(false);
    }

//-------------------- Interface Implementations --------------------//
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTentProgress();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTentProgress();
    }
}
