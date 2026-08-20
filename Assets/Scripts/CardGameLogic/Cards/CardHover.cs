using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[Header("Hover")]
	[SerializeField] private float hoverYOffset = 28f;
	[SerializeField] private float hoverScaleMultiplier = 1.12f;
	[SerializeField] private float hoverDuration = 0.1f;
	[SerializeField] private float returnDuration = 0.1f;

	private RectTransform rectTransform;
	private CardData cardData;
	private CardDrag cardDrag;

	private Vector2 baseAnchoredPosition;
	private Quaternion baseRotation;
	private Vector3 baseScale;
	private int baseSiblingIndex;
	private bool basePoseInitialized;
	private bool isHovered;
	private bool suppressHover;
	private Coroutine tweenRoutine;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		cardData = GetComponent<CardData>();
		cardDrag = GetComponent<CardDrag>();
		UpdateBasePose();
	}

	private void OnEnable()
	{
		EventManager.OnHandDrawStarted += HandleHandDrawStarted;
		EventManager.OnDrawnHand += HandleHandDrawn;
	}

	private void OnDisable()
	{
		EventManager.OnHandDrawStarted -= HandleHandDrawStarted;
		EventManager.OnDrawnHand -= HandleHandDrawn;
		StopTween();
	}

	public void UpdateBasePose()
	{
		if (rectTransform == null)
		{
			return;
		}

		baseAnchoredPosition = rectTransform.anchoredPosition;
		baseRotation = rectTransform.localRotation;
		baseScale = rectTransform.localScale;
		baseSiblingIndex = rectTransform.GetSiblingIndex();
		basePoseInitialized = true;

		if (isHovered)
		{
			isHovered = false;
			SetPoseInstant(baseAnchoredPosition, baseRotation, baseScale);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!CanHover())
		{
			return;
		}

		EnsureBasePose();
		isHovered = true;
		baseSiblingIndex = rectTransform.GetSiblingIndex();
		rectTransform.SetAsLastSibling();

		Vector2 targetPosition = baseAnchoredPosition + Vector2.up * hoverYOffset;
		Quaternion targetRotation = Quaternion.identity;
		Vector3 targetScale = baseScale * hoverScaleMultiplier;

		StartTween(targetPosition, targetRotation, targetScale, hoverDuration);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!isHovered)
		{
			return;
		}

		isHovered = false;
		EnsureBasePose();
		rectTransform.SetSiblingIndex(baseSiblingIndex);
		StartTween(baseAnchoredPosition, baseRotation, baseScale, returnDuration);
	}

	public void PrepareForDrag()
	{
		EnsureBasePose();
		isHovered = false;
		StopTween();
		rectTransform.SetSiblingIndex(baseSiblingIndex);
		SetPoseInstant(baseAnchoredPosition, baseRotation, baseScale);
	}

	public void DisableHover()
	{
		EnsureBasePose();
		isHovered = false;
		suppressHover = true;
		StopTween();
		UpdateBasePose();
		enabled = false;
	}

	private bool CanHover()
	{
		if (suppressHover)
		{
			return false;
		}

		if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
		{
			return false;
		}

		if (GameManager.Instance != null && !GameManager.Instance.IsPlayerTurn)
		{
			return false;
		}

		if (cardData != null && cardData.IsLocked)
		{
			return false;
		}

		if (cardDrag != null && cardDrag.IsDragging)
		{
			return false;
		}

		return true;
	}

	private void HandleHandDrawStarted()
	{
		suppressHover = true;
		isHovered = false;
		EnsureBasePose();
		SetPoseInstant(baseAnchoredPosition, baseRotation, baseScale);
		StopTween();
	}

	private void HandleHandDrawn()
	{
		suppressHover = false;
		UpdateBasePose();
	}

	private void EnsureBasePose()
	{
		if (!basePoseInitialized)
		{
			UpdateBasePose();
		}
	}

	private void StartTween(Vector2 targetPosition, Quaternion targetRotation, Vector3 targetScale, float duration)
	{
		StopTween();
		tweenRoutine = StartCoroutine(TweenToPose(targetPosition, targetRotation, targetScale, duration));
	}

	private System.Collections.IEnumerator TweenToPose(Vector2 targetPosition, Quaternion targetRotation, Vector3 targetScale, float duration)
	{
		if (rectTransform == null)
		{
			yield break;
		}

		Vector2 startPosition = rectTransform.anchoredPosition;
		Quaternion startRotation = rectTransform.localRotation;
		Vector3 startScale = rectTransform.localScale;

		if (duration <= 0f)
		{
			SetPoseInstant(targetPosition, targetRotation, targetScale);
			tweenRoutine = null;
			yield break;
		}

		float elapsed = 0f;
		while (elapsed < duration)
		{
			float t = Mathf.Clamp01(elapsed / duration);
			float eased = Mathf.SmoothStep(0f, 1f, t);

			rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPosition, targetPosition, eased);
			rectTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, eased);
			rectTransform.localScale = Vector3.LerpUnclamped(startScale, targetScale, eased);

			elapsed += Time.deltaTime;
			yield return null;
		}

		SetPoseInstant(targetPosition, targetRotation, targetScale);
		tweenRoutine = null;
	}

	private void SetPoseInstant(Vector2 position, Quaternion rotation, Vector3 scale)
	{
		if (rectTransform == null)
		{
			return;
		}

		rectTransform.anchoredPosition = position;
		rectTransform.localRotation = rotation;
		rectTransform.localScale = scale;
	}

	private void StopTween()
	{
		if (tweenRoutine != null)
		{
			StopCoroutine(tweenRoutine);
			tweenRoutine = null;
		}
	}
}
