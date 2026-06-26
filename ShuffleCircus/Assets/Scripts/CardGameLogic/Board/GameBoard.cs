using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public CardData[,] grid = new CardData[3, 5];
    [SerializeField] private bool isPlayerBoard;
    public bool IsPlayerBoard => isPlayerBoard;


    private void Awake()
    {
        AutoAssignSlotCoordinates(transform, isPlayerBoard);
    }

    private void AutoAssignSlotCoordinates(Transform root, bool isPlayerBoard)
    {
        CardSlot[] slots = root.GetComponentsInChildren<CardSlot>(true);
        int columns = grid.GetLength(1);

        if(slots.Length < grid.Length)
        {
            Debug.LogWarning($"GameBoard: '{root.name}' has only {slots.Length} CardSlots, but {grid.Length} are expected.");
        }

        for(int index = 0; index < slots.Length; index++)
        {
            int row = index / columns;
            int column = index % columns;

            if(row >= grid.GetLength(0))
            {
                break;
            }

            slots[index].SetCoordinates(row, column);
            slots[index].SetPlayerSlot(isPlayerBoard);
        }
    }

    public void PlaceCard(int row, int column, CardData card)
    {
        grid[row, column] = card;
    }

    public CardData GetCard(int row, int column)
    {
        return grid[row, column];
    }

    public void RemoveCard(int row, int column)
    {
        grid[row, column] = null;
    }

    public bool IsFull()
    {
        for(int row = 0; row < grid.GetLength(0); row++)
        {
            for(int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[row, col] == null) return false;
            }
        }
        return true;
    }
}
