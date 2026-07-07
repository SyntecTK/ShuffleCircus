using UnityEngine;

public class BackgroundPicker : MonoBehaviour
{
    [SerializeField] private Sprite bg_Singleplayer;
    [SerializeField] private Sprite bg_Multiplayer;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetBackground()
    {
        if (GameManager.Instance.IsMultiplayer)
        {
            _spriteRenderer.sprite = bg_Multiplayer;
        }
        else
        {
            _spriteRenderer.sprite = bg_Singleplayer;
        }
    }

}
