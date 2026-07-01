using TMPro;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [Header("GameBoards")]
    [SerializeField] private GameBoard _playerBoard;
    [SerializeField] private GameBoard _enemyBoard;

    [Header("Screens")]
    [SerializeField] private GameObject _resultScreen;

    [Header("TextFields")]
    [SerializeField] private TMP_Text _playerScoreText;
    [SerializeField] private TMP_Text _playerRow01Text;
    [SerializeField] private TMP_Text _playerRow02Text;
    [SerializeField] private TMP_Text _playerRow03Text;
    [SerializeField] private TMP_Text _enemyScoreText;
    [SerializeField] private TMP_Text _enemyRow01Text;
    [SerializeField] private TMP_Text _enemyRow02Text;   
    [SerializeField] private TMP_Text _enemyRow03Text;
    [SerializeField] private TMP_Text _playerDeckAmountText;
    [SerializeField] private TMP_Text _playerDiscardAmountText;
    [SerializeField] private TMP_Text _enemyDeckAmountText;
    [SerializeField] private TMP_Text _enemyDiscardAmountText;

    private void Start()
    {
        UpdateBoardUI();
    }


    private void OnEnable()
    {
        EventManager.OnBoardChanged += UpdateBoardUI;
        EventManager.OnTurnEnded += UpdateBoardUI;
        EventManager.OnGameOver += ShowResultScreen;
    }

    private void OnDisable()
    {
        EventManager.OnBoardChanged -= UpdateBoardUI;
        EventManager.OnTurnEnded -= UpdateBoardUI;
        EventManager.OnGameOver -= ShowResultScreen;
    }

    private void ShowResultScreen()
    {
        _resultScreen.SetActive(true);
    }

    public void UpdateBoardUI()
    {
        _playerScoreText.text = ScoringSystem.CalculateTotalScore(_playerBoard).ToString();
        _enemyScoreText.text = ScoringSystem.CalculateTotalScore(_enemyBoard).ToString();

        _playerRow01Text.text = ScoringSystem.CalculateRowScore(_playerBoard, 0).ToString();
        _playerRow02Text.text = ScoringSystem.CalculateRowScore(_playerBoard, 1).ToString();
        _playerRow03Text.text = ScoringSystem.CalculateRowScore(_playerBoard, 2).ToString();
        _enemyRow01Text.text = ScoringSystem.CalculateRowScore(_enemyBoard, 0).ToString();
        _enemyRow02Text.text = ScoringSystem.CalculateRowScore(_enemyBoard, 1).ToString();
        _enemyRow03Text.text = ScoringSystem.CalculateRowScore(_enemyBoard, 2).ToString();

        _playerDeckAmountText.text = DeckManager.Instance.GetDeckCount(true).ToString();
        _playerDiscardAmountText.text = DeckManager.Instance.GetDiscardCount(true).ToString();
        _enemyDeckAmountText.text = DeckManager.Instance.GetDeckCount(false).ToString();
        _enemyDiscardAmountText.text = DeckManager.Instance.GetDiscardCount(false).ToString();
    }
}
