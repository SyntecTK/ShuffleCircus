using UnityEngine;

public class ArrowSway : MonoBehaviour
{
    [SerializeField] private bool verticalSway = false;
    [SerializeField] private float swaySpeed = 2f;
    [SerializeField] private float swayAmount = 5f;
    [SerializeField] private bool isEnemyArrow = false;

    private RectTransform rectTransform;
    private Vector3 initialLocalPosition;
    private Vector2 initialAnchoredPosition;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialLocalPosition = transform.localPosition;

        if (rectTransform != null)
        {
            initialAnchoredPosition = rectTransform.anchoredPosition;
        }
    }

    void Update()
    {
        SwayAnimation();
    }

    private void SwayAnimation()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        if (isEnemyArrow)
        {
            sway = -sway;
        }

        if (rectTransform != null)
        {
            if (verticalSway)
            {
                rectTransform.anchoredPosition = new Vector2(initialAnchoredPosition.x, initialAnchoredPosition.y + sway);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(initialAnchoredPosition.x + sway, initialAnchoredPosition.y);
            }
        }
        else
        {
            if (verticalSway)
            {
                transform.localPosition = new Vector3(initialLocalPosition.x, initialLocalPosition.y + sway, initialLocalPosition.z);
            }
            else
            {
                transform.localPosition = new Vector3(initialLocalPosition.x + sway, initialLocalPosition.y, initialLocalPosition.z);
            }
        }
    }
}
