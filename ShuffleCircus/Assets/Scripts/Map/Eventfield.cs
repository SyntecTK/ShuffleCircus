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
    private int currentProgressStep = 0;


    private void Start()
    {
        eventSystem = GameObject.Find("EventSystem");
        mapRaycaster = GetComponentInParent<GraphicRaycaster>();
        tentProgressUI = transform.GetChild(0).gameObject;
        var allImages = tentProgressUI.GetComponentsInChildren<Image>(true);
        // Keep only top-level step images. This excludes child/icon images whose parent
        // already has an Image component so they won't be recolored when we update steps.
        tentProgressSteps = allImages.Where(img => img.transform.parent == tentProgressUI.transform).ToArray();

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
        Debug.Log(progress);
        for (int i = 0; i < tentProgressSteps.Length; i++)
        {
            if (tentProgressSteps[i].transform.childCount > 0)
                tentProgressSteps[i].transform.GetChild(0).gameObject.SetActive(false);

            if (i < progress)
            {
                tentProgressSteps[i].color = Color.green;
            }
            else
            {
                tentProgressSteps[i].color = Color.gray;
            }
        }
        tentProgressSteps[progress].color = Color.yellow;
        if(tentProgressSteps[progress].transform.childCount > 0)
            tentProgressSteps[progress].transform.GetChild(0).gameObject.SetActive(true);
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
