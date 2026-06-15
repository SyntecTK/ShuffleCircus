using UnityEngine;
using UnityEngine.EventSystems;

public class BoardSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private bool isPlayerSlot;
    [SerializeField] private GameBoard playerBoard;
    [SerializeField] private GameBoard enemyBoard;

    private GameBoard cachedParentBoard;

    public void Awake()
    {
        cachedParentBoard = isPlayerSlot ? playerBoard : enemyBoard;
    }
    public void OnDrop(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
