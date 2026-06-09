using TMPro;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [Header("TextFields")]
    [SerializeField] private TMP_Text _playerScoreText;
    [SerializeField] private TMP_Text _playerRow01Text;
    [SerializeField] private TMP_Text _playerRow02Text;
    [SerializeField] private TMP_Text _playerRow03Text;
    [SerializeField] private TMP_Text _enemyScoreText;
    [SerializeField] private TMP_Text _enemyRow01Text;
    [SerializeField] private TMP_Text _enemyRow02Text;   
    [SerializeField] private TMP_Text _enemyRow03Text;
    [SerializeField] private TMP_Text _deckAmountText;
    [SerializeField] private TMP_Text _discardAmountText;


    public void UpdateBoardUI()
    {
        // Implement logic to update the board UI based on the current game state
        // This could involve updating player scores, displaying current player turn, etc.
    }
}
