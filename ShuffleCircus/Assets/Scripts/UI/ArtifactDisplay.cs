using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;

public class ArtifactDisplay : MonoBehaviour
{
    [SerializeField] private int imgWidth;
    [SerializeField] private int imgHeight;

    private List<ArtifactData> activeArtifacts = new List<ArtifactData>();
    private List<GameObject> spawnedImages = new List<GameObject>();

    private void OnEnable()
    {
        EventManager.OnAddedArtifact += UpdateArtifactList;
    }

    private void OnDisable()
    {
        EventManager.OnAddedArtifact -= UpdateArtifactList;
    }

    private void Start()
    {
        UpdateArtifactList();
    }

    public void UpdateArtifactList()
    {
        activeArtifacts.Clear();
        foreach(ArtifactData artifact in ArtifactManager.Instance.ActiveArtifacts)
        {
            activeArtifacts.Add(artifact);
        }
        DisplayActiveArtifacts();
    }

    public void DisplayActiveArtifacts()
    {
        foreach(GameObject obj in spawnedImages)
        {
            Destroy(obj);
        }
        spawnedImages.Clear();

        Debug.Log("Active Artifacts: " + activeArtifacts.Count);

        foreach(ArtifactData artifact in activeArtifacts)
        {
            GameObject go = new GameObject("ArtifactIcon", typeof(RectTransform), typeof(Image));

            go.transform.SetParent(transform, false);

            Image img = go.GetComponent<Image>();
            img.sprite = artifact.Icon;
            img.preserveAspect = true;

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(imgWidth, imgHeight);

            spawnedImages.Add(go);
        }
        Debug.Log("Added Display");
    }
}
