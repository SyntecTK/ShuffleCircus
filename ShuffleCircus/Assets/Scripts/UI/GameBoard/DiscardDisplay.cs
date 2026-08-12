using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Data")]
    [SerializeField] private RectTransform cardPrefab;
    [SerializeField] private CardSpriteDataBase cardSpriteDataBase;

    [Header("References")]
    [SerializeField] private RectTransform discardImg;
    [SerializeField] private GameObject discardObj;
    [SerializeField] private GameObject display;

    private List<CardIdentity> discardList = new List<CardIdentity>();


    void OnEnable()
    {
        EventManager.OnTurnEnded += UpdateDiscards;
        EventManager.OnCardStolen += UpdateDiscards;
    }

    void OnDisable()
    {
        EventManager.OnTurnEnded -= UpdateDiscards;
        EventManager.OnCardStolen -= UpdateDiscards;
    }

    private void UpdateDiscards()
    {
        discardList = DeckManager.Instance.GetDiscardPile(true);
        Debug.Log(discardList.Count);
        DisplayDiscard(discardList, display.transform);
    }

    private void DisplayDiscard(List<CardIdentity> discard, Transform parent)
    {
        ClearChildren(parent);

        foreach (CardIdentity card in discard)
        {
            CardViewFactory.CreateCardView(cardPrefab, parent, card, cardSpriteDataBase, showFace: true, interactable: false);
        }
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

//---------------Interface implementation--------------------
    public void OnPointerEnter(PointerEventData eventData)
    {
        discardImg.transform.localScale = Vector3.one * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        discardImg.transform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        discardObj.SetActive(!discardObj.activeSelf);
    }
}

