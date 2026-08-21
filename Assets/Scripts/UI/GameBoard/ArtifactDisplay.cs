using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactDisplay : MonoBehaviour
{
    private const int SlotCount = 3;

    [SerializeField]private bool isPlayerDisplay = true;
    [Header("Fixed Display Slots (exactly 3)")]
    [SerializeField] private List<Image> slotImages = new List<Image>(SlotCount);

    private readonly List<ArtifactData> activeArtifacts = new List<ArtifactData>();
    private readonly List<Sprite> defaultSlotSprites = new List<Sprite>(SlotCount);

    private ArtifactSlot[] artifactSlots = new ArtifactSlot[SlotCount];
    private PulseScale[] slotPulseEffects = new PulseScale[SlotCount];

    private void Awake()
    {
        CacheDefaultSlotVisuals();
        for (int i = 0; i < SlotCount; i++)
        {
            if (i < slotImages.Count && slotImages[i] != null)
            {
                artifactSlots[i] = slotImages[i].GetComponent<ArtifactSlot>();
                slotPulseEffects[i] = slotImages[i].GetComponent<PulseScale>();
            }
        }
    }

    private void OnEnable()
    {
        EventManager.OnAddedArtifact += UpdateArtifactList;

        // Recover if serialized references changed and cached visuals are stale.
        if (defaultSlotSprites.Count < SlotCount)
        {
            CacheDefaultSlotVisuals();
        }
    }

    private void OnDisable()
    {
        EventManager.OnAddedArtifact -= UpdateArtifactList;
    }

    private void Start()
    {
        UpdateArtifactList();
    }

    private void CacheDefaultSlotVisuals()
    {
        defaultSlotSprites.Clear();

        for (int i = 0; i < SlotCount; i++)
        {
            if (i < slotImages.Count && slotImages[i] != null)
            {
                defaultSlotSprites.Add(slotImages[i].sprite);
            }
            else
            {
                defaultSlotSprites.Add(null);
            }
        }
    }

    private bool HasArtifactIconAtSlot(int slotIndex)
    {
        return slotIndex < activeArtifacts.Count && activeArtifacts[slotIndex] != null && activeArtifacts[slotIndex].Icon != null;
    }

    public void UpdateArtifactList()
    {
        if(!isPlayerDisplay) return;
        activeArtifacts.Clear();

        if (ArtifactManager.Instance != null)
        {
            foreach (ArtifactData artifact in ArtifactManager.Instance.ActiveArtifacts)
            {
                activeArtifacts.Add(artifact);
            }
        }

        DisplayActiveArtifacts();
    }

    public void DisplayActiveArtifacts()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (i >= slotImages.Count || slotImages[i] == null)
            {
                continue;
            }

            Image slot = slotImages[i];
            slot.preserveAspect = true;

            bool hasIcon = HasArtifactIconAtSlot(i);

            if (hasIcon)
            {
                slot.sprite = activeArtifacts[i].Icon;
                artifactSlots[i].BindArtifact(activeArtifacts[i]);
            }
            else
            {
                slot.sprite = i < defaultSlotSprites.Count ? defaultSlotSprites[i] : null;
            }

            if (slotPulseEffects[i] != null)
            {
                slotPulseEffects[i].enabled = !hasIcon;
            }
        }

        if (activeArtifacts.Count > SlotCount)
        {
            Debug.LogWarning($"ArtifactDisplay supports only {SlotCount} slots. Extra artifacts are not shown.");
        }
    }
}
