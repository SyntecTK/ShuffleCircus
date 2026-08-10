using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayDisabler : MonoBehaviour, IPointerClickHandler
{
    private RectTransform popup;

    void Start()
    {
        popup = transform.GetChild(0).GetComponent<RectTransform>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(popup, eventData.position, eventData.pressEventCamera)) return;
        gameObject.SetActive(false);
    }
} 
