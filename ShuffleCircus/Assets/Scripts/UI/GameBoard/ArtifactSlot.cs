using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ArtifactSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameplayManager gameplayManager;
    private ArtifactData storedArtifact;

    private void Awake()
    {
        gameplayManager = FindFirstObjectByType<GameplayManager>();

    }

    public void BindArtifact(ArtifactData artifact)
    {
        storedArtifact = artifact;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (storedArtifact == null || gameplayManager == null) return;
        gameplayManager.Tooltip.ShowTooltip(storedArtifact.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (storedArtifact == null || gameplayManager == null) return;
        gameplayManager.Tooltip.HideTooltip();
    }
}

