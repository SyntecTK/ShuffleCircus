using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutImage : MonoBehaviour
{
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
        float duration = 6f;
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / duration);
            fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }
}
