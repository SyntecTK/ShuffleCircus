using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BoardUI : MonoBehaviour
{
    [Header("GameBoards")]
    [SerializeField] private GameBoard _playerBoard;
    [SerializeField] private GameBoard _enemyBoard;

    [Header("Screens")]
    [SerializeField] private GameObject _resultScreen;
    [SerializeField] private GameObject _cheatSheet;
    [SerializeField] private RectTransform _cheatSheetContent;
    [SerializeField] private GameObject _pauseMenu;

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
    [SerializeField] private TMP_Text _cardsPlayerPlayedThisTurnText;

    [Header("Debug")]
    [SerializeField] private TMP_Text cpuLvlText;

    private void Start()
    {
        UpdateBoardUI();
    }

    private void OnEnable()
    {
        EventManager.OnBoardChanged += UpdateBoardUI;
        EventManager.OnTurnEnded += UpdateBoardUI;
        EventManager.OnGameOver += ShowResultScreen;
        EventManager.OnPause += ShowPauseMenu;
    }

    private void OnDisable()
    {
        EventManager.OnBoardChanged -= UpdateBoardUI;
        EventManager.OnTurnEnded -= UpdateBoardUI;
        EventManager.OnGameOver -= ShowResultScreen;
        EventManager.OnPause -= ShowPauseMenu;
    }

    private void ShowResultScreen()
    {
        _resultScreen.SetActive(true);
        bool playerisWinner = ScoringSystem.CalculateTotalScore(_playerBoard) > ScoringSystem.CalculateTotalScore(_enemyBoard);
        _resultScreen.GetComponent<ResultScreen>().SetWinner(playerisWinner);
    }

    private void ShowPauseMenu()
    {
        Time.timeScale = 0;
        if(_pauseMenu.activeSelf)
        {
            _pauseMenu.SetActive(false);
            return;
        }
        _pauseMenu.SetActive(true);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1;
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    public void ReturnToGame()
    {
        Time.timeScale = 1;
        _pauseMenu.SetActive(false);
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

        cpuLvlText.text = GameManager.Instance.State.AIDifficultyLevel.ToString();

        if(GameManager.Instance.State.GameMode == GameMode.Multiplayer)
        {
            if(GameManager.Instance.IsPlayerTurn)
            {
                _cardsPlayerPlayedThisTurnText.text = GameManager.Instance.CardsRemainingThisTurn.ToString();
            }
        }
        else
        {
            _cardsPlayerPlayedThisTurnText.text = GameManager.Instance.IsPlayerTurn
                ? GameManager.Instance.CardsRemainingThisTurn.ToString()
                : GameManager.Instance.MaxCardsAllowedPerTurn.ToString();
        }
    }

    public void PassTurn()
    {
        if(GameManager.Instance.State.GameMode == GameMode.Singleplayer)
        {
            if (GameManager.Instance != null && 
                (GameManager.Instance.IsGameOver || 
                !GameManager.Instance.IsPlayerTurn))
            {
                return;
            }
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            {
                return;
            }
        }

        if(InputManager.Instance.CanPassTurn)
        {
            GameManager.Instance.EndTurn();
            EventManager.BoardChanged();
        }
    }

    public void ToggleCheatSheet()
    {
        _cheatSheet.SetActive(_cheatSheet.activeSelf ? false : true);
    }

    private void Update()
    {
        if (_cheatSheet != null && _cheatSheet.activeSelf && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!IsPointerOverCheatSheetContent())
            {
                _cheatSheet.SetActive(false);
            }
        }
    }

    private bool IsPointerOverCheatSheetContent()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        Transform contentTransform = _cheatSheetContent != null
            ? _cheatSheetContent.transform
            : _cheatSheet.transform;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.transform == contentTransform ||
                result.gameObject.transform.IsChildOf(contentTransform))
            {
                return true;
            }
        }

        return false;
    }
}
