using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private GameBoard _playerBoard;
    [SerializeField] private GameBoard _enemyBoard;

    private void Awake()
    {
        GameManager.Instance.SetPlayerTurn(true);
    }

    private void OnEnable()
    {
        EventManager.OnCardDropped += HandleCardPlayed;
    }

    private void OnDisable()
    {
        EventManager.OnCardDropped -= HandleCardPlayed;
    }

    private void HandleCardPlayed(int row, int column, bool playedByPlayer)
    {
        GameBoard board = playedByPlayer ? _playerBoard : _enemyBoard;
        CardData card = board.GetCard(row, column);

        if (card == null)
        {
            Debug.LogWarning("GameplayManager: No card found in the specified slot.");
            return;
        }

        StealMatchingRankInRow(row, card.RankValue, playedByPlayer);
        if (board.IsFull())
        {
            SceneLoader.Instance.UnloadAdditiveScene("GameBoard");
        }
    }

    public void StealMatchingRankInRow(int row, int rank, bool playedByPlayer)
    {
        if (DeckManager.Instance == null)
        {
            Debug.LogWarning("GameplayManager: DeckManager instance not found.");
            return;
        }

        GameBoard opponentBoard = playedByPlayer ? _enemyBoard : _playerBoard;
        if (opponentBoard == null)
        {
            Debug.LogWarning("GameplayManager: Opponent board reference is missing.");
            return;
        }

        int columnCount = opponentBoard.grid.GetLength(1);
        for (int column = 0; column < columnCount; column++)
        {
            CardData targetCard = opponentBoard.GetCard(row, column);
            if (targetCard == null || targetCard.RankValue != rank)
            {
                continue;
            }

            opponentBoard.RemoveCard(row, column);
            DeckManager.Instance.AddToDiscard(targetCard.Identity, playedByPlayer);
            Destroy(targetCard.gameObject);
        }
    }

    public GameBoard GetPlayerBoard()
    {
        return _playerBoard;
    }

    public GameBoard GetEnemyBoard()
    {
        return _enemyBoard;
    }
}
