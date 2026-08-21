using UnityEngine;

public class PulseScale : MonoBehaviour
{
    [Range(0.7f, 1f)]
    [SerializeField] private float minScale = 0.92f;
    [Range(0.5f, 6f)]
    [SerializeField] private float pulseSpeed = 2.2f;
    [SerializeField] private float phaseOffset = 0f;

    private Vector3 baseScale;

    private void OnEnable()
    {
        baseScale = transform.localScale;
    }

    private void OnDisable()
    {
        transform.localScale = baseScale;
    }

    private void Update()
    {
        float wave = (Mathf.Sin((Time.unscaledTime * pulseSpeed) + phaseOffset) + 1f) * 0.5f;
        float scaleMultiplier = Mathf.Lerp(1f, minScale, wave);
        transform.localScale = baseScale * scaleMultiplier;
    }
}
