using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class ResultScreen : MonoBehaviour
{
    private const int selectionSlots = 2;

    [Header("References")]
    [SerializeField] private TMP_Text winText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Button nextButton;
    //[SerializeField] private Button mapButton;

    [Header("Minimizing")]
    [SerializeField] private RectTransform resultPanel;
    [SerializeField] private GameObject _uiContainer;

    [Header("ArtifactSelector")]
    [SerializeField] private GameObject selectionContainer;
    [SerializeField] private GameObject selectionPrefab;
    private ArtifactData selectedArtifact;
    private bool canClaimArtifact;

    private float _originalHeight;
    private bool _isMinimized = false;
    private bool _isPlayerWinner;
    private bool _isAdvancingToNextGame = false;

    void Start()
    {
        _originalHeight = resultPanel.rect.height;
    }

    private void OnEnable()
    {
        _isAdvancingToNextGame = false;
        ClearArtifactSelections();  
    }

    private void ClearArtifactSelections()
    {
        selectedArtifact = null;
        rewardText.text = "";

        for (int i = selectionContainer.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(selectionContainer.transform.GetChild(i).gameObject);
        }
    }

    private void FillArtifactSelections()
    {
        ClearArtifactSelections();

        rewardText.text = "Rewards (choose one):";

        if (ArtifactManager.Instance == null)
        {
            Debug.LogWarning("ArtifactManager instance not found. Cannot fill artifact selections.");
            return;
        }

        List<ArtifactData> availableArtifacts = new List<ArtifactData>();
        foreach (ArtifactData artifact in ArtifactManager.Instance.AllArtifacts)
        {
            if (!ArtifactManager.Instance.ActiveArtifacts.Contains(artifact))
            {
                availableArtifacts.Add(artifact);
            }
        }

        int slotsToFill = Mathf.Min(selectionSlots, availableArtifacts.Count);

        for (int i = 0; i < slotsToFill; i++)
        {
            GameObject selection = Instantiate(selectionPrefab, selectionContainer.transform);
            TMP_Text nameText = selection.transform.GetChild(0).GetComponent<TMP_Text>();
            Image spriteImage = selection.transform.GetChild(1).GetComponent<Image>();
            TMP_Text descriptionText = selection.transform.GetChild(2).GetComponent<TMP_Text>();

            int randomIndex = Random.Range(0, availableArtifacts.Count);
            ArtifactData randomArtifact = availableArtifacts[randomIndex];
            availableArtifacts.RemoveAt(randomIndex);

            nameText.text = randomArtifact.Name;
            spriteImage.sprite = randomArtifact.Icon;
            descriptionText.text = randomArtifact.Description;

            ArtifactSelector selector = selection.GetComponent<ArtifactSelector>();
            if (selector != null)
            {
                selector.Configure(randomArtifact, this);
            }
        }
    }

    public void SelectArtifact(ArtifactData artifact)
    {
        selectedArtifact = artifact;
        if(!nextButton.gameObject.activeSelf)
        {
            nextButton.gameObject.SetActive(true);
            //mapButton.gameObject.SetActive(true);
        }
    }

    private void Minimize()
    {
        resultPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30f);
        _uiContainer.SetActive(false);
    }  

    private void Restore()
    {
        if (resultPanel == null) return;
        resultPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _originalHeight);
        _uiContainer.SetActive(true);
    }

    public void ToggleResultPanel()
    {
        if (_isMinimized)
        {
            Restore();
        }
        else
        {
            Minimize();
        }
        _isMinimized = !_isMinimized;
    }

    public void SetWinner(bool isWinner)
    {
        if (winText == null) return;
        _isPlayerWinner = isWinner;
        canClaimArtifact = _isPlayerWinner;
        _isAdvancingToNextGame = false;
        nextButton.interactable = true;
        bool isFinalBattle = GameManager.Instance != null &&
            GameManager.Instance.CurrentBattleCount >= GameManager.Instance.MaxBattlesAllowed;

        nextButton.onClick.RemoveAllListeners();

        if (isFinalBattle)
        {
            canClaimArtifact = false;
            ClearArtifactSelections();
            rewardText.text = "Thanks for playing";
            winText.text = _isPlayerWinner ? "You Win!" : "You Lose!";
            nextButton.onClick.AddListener(QuitToMainMenu);
            return;
        }

        if(_isPlayerWinner)
        {
            FillArtifactSelections();
            GameManager.Instance.IncreaseTentProgress(GameManager.Instance.SelectedEventFieldID);
            winText.text = "You Win!";
            if (GameManager.Instance == null || GameManager.Instance.CurrentBattleCount < GameManager.Instance.MaxBattlesAllowed)
            {
                nextButton.GetComponentInChildren<TMP_Text>().text = "Next Game";
                nextButton.onClick.AddListener(StartNextGame);
            }
            else
            {
                nextButton.enabled = false;
            }
        }
        else
        {
            ClearArtifactSelections();
            winText.text = "You Lose!";
            nextButton.GetComponentInChildren<TMP_Text>().text = "Retry";
            nextButton.onClick.AddListener(RetryGame);
            
        }
    }

    private void RetryGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayerTurn(true);
        }

        if (DeckManager.Instance != null)
        {
            DeckManager.Instance.ResetDecks();
        }

        SceneLoader.Instance.LoadScene("GameBoard");
    }

    public void StartNextGame()
    {
        if (!canClaimArtifact)
        {
            RetryGame();
            return;
        }

        // Guard against double invocation (e.g. double-click while the scene is still loading),
        // which previously caused AIDifficultyLevel / battle counter / tent progress to be
        // incremented more than once for a single "next game" transition.
        if (_isAdvancingToNextGame) return;

        if (selectedArtifact != null)
        {
            _isAdvancingToNextGame = true;
            nextButton.interactable = false;

            ArtifactManager.Instance.AddActiveArtifact(selectedArtifact);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetPlayerTurn(true);
            }

            DeckManager.Instance.ResetDecks();
            GameManager.Instance.State.IncreaseAIDifficultyLevel();
            GameManager.Instance.IncreaseBattleCounter();
            // Note: tent progress for this win is already increased in SetWinner(); do not
            // increase it again here.
            SceneLoader.Instance.LoadScene("GameBoard");
        }
    }

    public void QuitToMainMenu()
    {
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    // Unload the additively loaded GameBoard scene and return to the underlying map scene.
    // Called by nextButton in final-battle flow (see where LoadMap is added as listener).
    public void LoadMap()
    {
        // If GameBoard was loaded additively from the map, unload it and when finished
        // re-enable the EventSystem and map raycasters so the map UI is interactive again.
        Scene scene = SceneManager.GetSceneByName("GameBoard");
        if (scene.IsValid() && scene.isLoaded)
        {
            AsyncOperation op = SceneManager.UnloadSceneAsync("GameBoard");
            if (op != null)
            {
                op.completed += _ => OnMapReturned();
            }
            else
            {
                // If unload failed to return an AsyncOperation, try a direct re-enable fallback.
                OnMapReturned();
            }
        }
        else
        {
            // If GameBoard isn't loaded, still ensure map interactivity.
            OnMapReturned();
        }

        DeckManager.Instance.ResetDecks();
        ArtifactManager.Instance.AddActiveArtifact(selectedArtifact);
        // Clear selected event so map doesn't keep stale selection.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectedEventFieldID = -1;
        }
    }

    private void OnMapReturned()
    {
        // Re-enable EventSystem and GraphicRaycasters in all loaded scenes.
        // Note: GameObject.Find doesn't return inactive objects, so we iterate root
        // objects of each loaded scene to find the EventSystem even if it's inactive.
        int sceneCount = SceneManager.sceneCount;
        for (int s = 0; s < sceneCount; s++)
        {
            var scene = SceneManager.GetSceneAt(s);
            if (!scene.IsValid()) continue;

            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                // Find and enable EventSystem (by name) even if it's inactive.
                if (root.name == "EventSystem")
                {
                    if (!root.activeInHierarchy)
                        root.SetActive(true);
                }

                // Re-enable all GraphicRaycasters under this root (including inactive ones).
                var raycasters = root.GetComponentsInChildren<UnityEngine.UI.GraphicRaycaster>(true);
                foreach (var r in raycasters)
                {
                    if (r != null && !r.enabled)
                        r.enabled = true;
                }
            }
        }
    }
}

