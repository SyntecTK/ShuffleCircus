using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutImage : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 6f;
    [SerializeField] private float fadeDelay = 1f;
    [SerializeField, Range(1f, 10f)] private float fadeAccelerationPower = 4f;
    private Image fadeImage;
    void Start()
    {
        fadeImage = GetComponent<Image>();
        if(fadeImage == null)
        {
            Debug.LogWarning("Object needs an Image Component!");
        }
        else
        {
            StartCoroutine(FadeOutRoutine());
        }
    }

    IEnumerator FadeOutRoutine()
    {
        float duration = fadeDuration;
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;

        yield return new WaitForSeconds(fadeDelay); 
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = Mathf.Pow(t, fadeAccelerationPower);
            float alpha = Mathf.Lerp(startColor.a, 0f, easedT);
            fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        fadeImage.gameObject.SetActive(false);
    }
}
