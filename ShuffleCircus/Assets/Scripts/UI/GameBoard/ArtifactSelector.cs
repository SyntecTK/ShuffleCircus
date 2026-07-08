using UnityEngine;
using UnityEngine.UI;

public class ArtifactSelector : MonoBehaviour
{
    [SerializeField] private Color selectedColor;
    private Color defaultColor;
    private Image buttonImage;
    private Button button;
    private ArtifactData assignedArtifact;
    private ResultScreen owner;

    private static ArtifactSelector currentlySelected;
    
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();
        defaultColor = buttonImage.color;

        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
        }
    }

    public void Configure(ArtifactData artifact, ResultScreen resultScreen)
    {
        assignedArtifact = artifact;
        owner = resultScreen;
    }

    private void OnDisable()
    {
        if (currentlySelected == this)
        {
            currentlySelected = null;
        }
    }

    public void PickArtifact()
    {
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.DeselectArtifact();
        }

        buttonImage.color = selectedColor;
        currentlySelected = this;

        ArtifactData selected = assignedArtifact;
        if (owner != null)
        {
            owner.SelectArtifact(selected);
        }

        if (selected != null)
        {
            Debug.Log($"Artifact '{selected.Name}' selected.");
        }
        else
        {
            Debug.LogWarning("Attempted to pick a null artifact.");
        }
    }

    private void HandleClick()
    {
        PickArtifact();
    }

    public void DeselectArtifact()
    {
        buttonImage.color = defaultColor;

        if (currentlySelected == this)
        {
            currentlySelected = null;
        }
    }
}
