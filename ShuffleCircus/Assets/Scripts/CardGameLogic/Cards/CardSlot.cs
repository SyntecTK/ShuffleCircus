using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private bool isPlayerSlot;
    [SerializeField] private int row;
    [SerializeField] private int column;
    private GameBoard board;


    private void Awake()
    {
        board = FindBoardInParents();
        if(board == null)
        {
            Debug.LogError("CardSlot: Board not found in parent.");
        }
    }
    private GameBoard FindBoardInParents()
    {
        Transform current = transform.parent;

        while(current != null)
        {
            GameBoard foundBoard = current.GetComponent<GameBoard>();
            if(foundBoard != null)
            {
                return foundBoard;
            }

            current = current.parent;
        }

        return null;
    }

    public void SetPlayerSlot(bool isPlayerSlot)
    {
        this.isPlayerSlot = isPlayerSlot;
    }

    public void SetCoordinates(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public void RemoveCard(CardData card)
    {
        if(board == null || card == null)
        {
            Debug.LogWarning("CardSlot: Cannot remove card. Board or card is null.");
            return;
        }

        if(board.GetCard(row, column) == card)
        {
            board.RemoveCard(row, column);
        }
    }

    public void RestoreCard(CardData card)
    {
        board.PlaceCard(row, column, card);
        EventManager.CardDropped(row, column, isPlayerSlot);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (GameManager.Instance.IsGameOver) return;

        CardData card = eventData.pointerDrag != null ? eventData.pointerDrag.GetComponent<CardData>() : null;
        CardDrag cardDrag = eventData.pointerDrag != null ? eventData.pointerDrag.GetComponent<CardDrag>() : null;
        HandManager sourceHand = eventData.pointerDrag != null ? eventData.pointerDrag.GetComponentInParent<HandManager>() : null;

        if(card == null || cardDrag == null) return;

        if(GameManager.Instance.IsPlayerTurn && !isPlayerSlot)
        {
            Debug.LogWarning("Player cannot drop card in opponent's slot.");
            cardDrag.RestoreToOriginalPosition();
            sourceHand.RearrangeCards();
            return;
        }

        if(!GameManager.Instance.IsPlayerTurn && isPlayerSlot)
        {
            Debug.LogWarning("Opponent cannot drop card in player's slot.");
            cardDrag.RestoreToOriginalPosition();
            sourceHand.RearrangeCards();
            return;
        }

        if(!GameManager.Instance.CanPlayCardThisTurn())
        {
            Debug.LogWarning("No more cards can be played this turn.");
            cardDrag.RestoreToOriginalPosition();
            sourceHand.RearrangeCards();
            return;
        }

        CardData existingCard = board.GetCard(row, column);
        if(existingCard != null && existingCard != card)
        {
            Debug.LogWarning($"CardSlot: Slot at ({row}, {column}) is already occupied.");
            cardDrag.RestoreToOriginalPosition();
            sourceHand.RearrangeCards();
            return;
        }

        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;
        card.transform.localScale = Vector3.one;

        CardHover cardHover = card.GetComponent<CardHover>();
        if (cardHover != null)
        {
            cardHover.DisableHover();
        }

        sourceHand?.RemoveCardFromHand(card.GetComponent<RectTransform>());

        board.PlaceCard(row, column, card);
        GameManager.Instance.TryRegisterPlayedCard();
        cardDrag.MarkDropped();
        
        EventManager.CardDropped(row, column, isPlayerSlot);
        EventManager.BoardChanged();
    }
}
