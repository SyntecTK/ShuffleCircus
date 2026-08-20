using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class BackgroundScroll : MonoBehaviour
{
    [Tooltip("Scroll-Geschwindigkeit in UV-Einheiten pro Sekunde (X und Y). Werte um 0.01 sind meist schon deutlich sichtbar.")]
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.01f, 0f);

    private RawImage rawImage;
    private Vector2 offset;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        offset += scrollSpeed * Time.deltaTime;

        Rect uv = rawImage.uvRect;
        uv.position = offset;
        rawImage.uvRect = uv;
    }
}
