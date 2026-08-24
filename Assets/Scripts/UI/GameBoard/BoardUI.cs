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
    [SerializeField] private GameObject _afkWarningScreen;
    [SerializeField] private TMP_Text _afkWarningText;

    private const float AfkIdleThreshold = 60f;
    private const float AfkCountdownDuration = 10f;

    private float _afkIdleTimer;
    private float _afkCountdownTimer;
    private bool _afkWarningActive;
    private Vector2 _lastMousePosition;

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
        if (Mouse.current != null)
        {
            _lastMousePosition = Mouse.current.position.ReadValue();
        }
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
            Time.timeScale = 1;
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

        HandleAfkDetection();
    }

    private void HandleAfkDetection()
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        bool mouseActive = currentMousePosition != _lastMousePosition
            || Mouse.current.leftButton.wasPressedThisFrame
            || Mouse.current.rightButton.wasPressedThisFrame;
        _lastMousePosition = currentMousePosition;

        if (_afkWarningActive)
        {
            _afkCountdownTimer -= Time.unscaledDeltaTime;
            UpdateAfkWarningText();
            if (_afkCountdownTimer <= 0f)
            {
                ReturnToMenuFromAfk();
            }
            return;
        }

        bool blockedByOtherScreen = (_pauseMenu != null && _pauseMenu.activeSelf)
            || (_resultScreen != null && _resultScreen.activeSelf)
            || (_cheatSheet != null && _cheatSheet.activeSelf);

        if (mouseActive || blockedByOtherScreen)
        {
            _afkIdleTimer = 0f;
            return;
        }

        _afkIdleTimer += Time.unscaledDeltaTime;
        if (_afkIdleTimer >= AfkIdleThreshold)
        {
            ShowAfkWarning();
        }
    }

    private void ShowAfkWarning()
    {
        _afkWarningActive = true;
        _afkCountdownTimer = AfkCountdownDuration;
        UpdateAfkWarningText();
        _afkWarningScreen.SetActive(true);
    }

    private void UpdateAfkWarningText()
    {
        int secondsLeft = Mathf.CeilToInt(Mathf.Max(_afkCountdownTimer, 0f));
        _afkWarningText.text = $"AFK warning! Returning to main menu in {secondsLeft}s";
    }

    public void CancelAfkWarning()
    {
        _afkWarningActive = false;
        _afkIdleTimer = 0f;
        _afkCountdownTimer = 0f;
        _afkWarningScreen.SetActive(false);
    }

    public void PauseGame()
    {
        EventManager.Pause();
    }

    private void ReturnToMenuFromAfk()
    {
        _afkWarningActive = false;
        _afkWarningScreen.SetActive(false);
        SceneLoader.Instance.LoadScene("MainMenu");
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
