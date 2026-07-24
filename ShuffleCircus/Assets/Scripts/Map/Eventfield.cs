using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Eventfield : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int eventID = 0;
    private GameObject eventSystem;
    private GraphicRaycaster mapRaycaster;
    private GameObject tentProgressUI;
    private Image[] tentProgressSteps;
    private GameObject stepsContainer;


    private void Start()
    {
        eventSystem = GameObject.Find("EventSystem");
        mapRaycaster = GetComponentInParent<GraphicRaycaster>();
        tentProgressUI = transform.GetChild(0).gameObject;
        var allImages = tentProgressUI.GetComponentsInChildren<Image>();
        tentProgressSteps = allImages.Where(img => img.gameObject != tentProgressUI).ToArray();

        if (eventSystem != null && !eventSystem.activeInHierarchy)
        {
            EnableEventSystem();
        }
    }

    public void OnLevelSelected()
    {
        DisableEventSystem();
        DisableMapRaycasts();

        GameManager.Instance.SelectedEventFieldID = eventID;
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
        UpdateTentProgressUI();
        tentProgressUI.SetActive(true);
    }

    private void HideTentProgress()
    {
        tentProgressUI.SetActive(false);
    }

    private void UpdateTentProgressUI()
    {
        int progress = GameManager.Instance.GetTentProgress(eventID);

        for (int i = 0; i < tentProgressSteps.Length; i++)
        {
            if (i < progress)
            {
                tentProgressSteps[i].color = Color.green; // Filled step
            }
            else
            {
                tentProgressSteps[i].color = Color.gray; // Empty step
            }
        }
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
