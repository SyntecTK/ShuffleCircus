using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactDisplay : MonoBehaviour
{
    private const int SlotCount = 3;

    [SerializeField]private bool isPlayerDisplay = true;
    [Header("Fixed Display Slots (exactly 3)")]
    [SerializeField] private List<Image> slotImages = new List<Image>(SlotCount);

    [Header("Empty Slot Hover Animation")]
    [Range(0.7f, 1f)]
    [SerializeField] private float emptySlotMinScale = 0.92f;
    [Range(0.5f, 6f)]
    [SerializeField] private float emptySlotPulseSpeed = 2.2f;
    [SerializeField] private float slotPhaseOffset = 0.35f;

    private readonly List<ArtifactData> activeArtifacts = new List<ArtifactData>();
    private readonly List<Sprite> defaultSlotSprites = new List<Sprite>(SlotCount);
    private readonly List<Vector3> defaultSlotScales = new List<Vector3>(SlotCount);

    private ArtifactSlot[] artifactSlots = new ArtifactSlot[SlotCount];

    private Coroutine emptySlotAnimationRoutine;


    private void Awake()
    {
        CacheDefaultSlotVisuals();
        for (int i = 0; i < SlotCount; i++)
        {
            if (i < slotImages.Count && slotImages[i] != null)
            {
                artifactSlots[i] = slotImages[i].GetComponent<ArtifactSlot>();
            }
        }
    }

    private void OnEnable()
    {
        EventManager.OnAddedArtifact += UpdateArtifactList;

        // Recover if serialized references changed and cached visuals are stale.
        if (defaultSlotScales.Count < SlotCount || defaultSlotSprites.Count < SlotCount)
        {
            CacheDefaultSlotVisuals();
        }

        StartEmptySlotAnimation();
    }

    private void OnDisable()
    {
        EventManager.OnAddedArtifact -= UpdateArtifactList;
        StopEmptySlotAnimation();
        ResetAllSlotScales();
    }

    private void Start()
    {
        UpdateArtifactList();
    }

    private void CacheDefaultSlotVisuals()
    {
        defaultSlotSprites.Clear();
        defaultSlotScales.Clear();

        for (int i = 0; i < SlotCount; i++)
        {
            if (i < slotImages.Count && slotImages[i] != null)
            {
                defaultSlotSprites.Add(slotImages[i].sprite);
                defaultSlotScales.Add(slotImages[i].rectTransform.localScale);
            }
            else
            {
                defaultSlotSprites.Add(null);
                defaultSlotScales.Add(Vector3.one);
            }
        }
    }

    private void StartEmptySlotAnimation()
    {
        if (emptySlotAnimationRoutine == null)
        {
            emptySlotAnimationRoutine = StartCoroutine(AnimateEmptySlots());
        }
    }

    private void StopEmptySlotAnimation()
    {
        if (emptySlotAnimationRoutine != null)
        {
            StopCoroutine(emptySlotAnimationRoutine);
            emptySlotAnimationRoutine = null;
        }
    }

    private IEnumerator AnimateEmptySlots()
    {
        while (true)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                if (i >= slotImages.Count || slotImages[i] == null)
                {
                    continue;
                }

                Image slot = slotImages[i];
                Vector3 baseScale = i < defaultSlotScales.Count ? defaultSlotScales[i] : Vector3.one;

                if (HasArtifactIconAtSlot(i))
                {
                    slot.rectTransform.localScale = baseScale;
                    continue;
                }

                float wave = (Mathf.Sin((Time.unscaledTime * emptySlotPulseSpeed) + (i * slotPhaseOffset)) + 1f) * 0.5f;
                float scaleMultiplier = Mathf.Lerp(1f, emptySlotMinScale, wave);
                slot.rectTransform.localScale = baseScale * scaleMultiplier;
            }

            yield return null;
        }
    }

    private bool HasArtifactIconAtSlot(int slotIndex)
    {
        return slotIndex < activeArtifacts.Count && activeArtifacts[slotIndex] != null && activeArtifacts[slotIndex].Icon != null;
    }

    private void ResetAllSlotScales()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (i >= slotImages.Count || slotImages[i] == null)
            {
                continue;
            }

            Vector3 baseScale = i < defaultSlotScales.Count ? defaultSlotScales[i] : Vector3.one;
            slotImages[i].rectTransform.localScale = baseScale;
        }
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

            if (HasArtifactIconAtSlot(i))
            {
                slot.sprite = activeArtifacts[i].Icon;
                artifactSlots[i].BindArtifact(activeArtifacts[i]);
            }
            else
            {
                slot.sprite = i < defaultSlotSprites.Count ? defaultSlotSprites[i] : null;
            }
        }

        if (activeArtifacts.Count > SlotCount)
        {
            Debug.LogWarning($"ArtifactDisplay supports only {SlotCount} slots. Extra artifacts are not shown.");
        }
    }
}
