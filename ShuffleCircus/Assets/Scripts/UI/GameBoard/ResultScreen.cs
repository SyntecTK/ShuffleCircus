using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResultScreen : MonoBehaviour
{
    private const int selectionSlots = 2;

    [Header("Minimizing")]
    [SerializeField] private RectTransform resultPanel;
    [SerializeField] private GameObject _uiContainer;

    [Header("ArtifactSelector")]
    [SerializeField] private GameObject selectionContainer;
    [SerializeField] private GameObject selectionPrefab;
    private ArtifactData selectedArtifact;

    private float _originalHeight;
    private bool _isMinimized = false;

    void Start()
    {
        _originalHeight = resultPanel.rect.height;
    }

    private void OnEnable()
    {
        FillArtifactSelections();
    }


    private void FillArtifactSelections()
    {
        selectedArtifact = null;

        for (int i = selectionContainer.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(selectionContainer.transform.GetChild(i).gameObject);
        }

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

    public void StartNextGame()
    {
        if (selectedArtifact != null)
        {
            ArtifactManager.Instance.AddActiveArtifact(selectedArtifact);
        }

        DeckManager.Instance.ResetDecks();
        GameManager.Instance.State.IncreaseAIDifficultyLevel();
        SceneLoader.Instance.LoadScene("GameBoard");
    }
}
