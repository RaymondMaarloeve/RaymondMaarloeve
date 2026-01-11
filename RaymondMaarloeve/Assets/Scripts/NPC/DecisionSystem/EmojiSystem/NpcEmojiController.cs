using UnityEngine;
//using DG.Tweening; // Opcjonalnie, jeœli u¿ywasz DOTween do ³adnych animacji pojawiania siê

public class NpcEmojiController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer emojiRenderer;
    [SerializeField] private NpcActionIcons iconDatabase;

    [Header("Settings")]
    [SerializeField] private float heightOffset = 2.5f; // Wysokoœæ nad NPC
    [SerializeField] private bool billboardEffect = true; // Czy ma patrzeæ na kamerê?

    private Transform _mainCameraTransform;

    void Start()
    {
        if (Camera.main != null)
            _mainCameraTransform = Camera.main.transform;

        // Ustawienie pozycji nad g³ow¹
        emojiRenderer.transform.localPosition = new Vector3(0, heightOffset, 0);

        // Na starcie ukrywamy
        HideEmoji();
    }

    void LateUpdate()
    {
        // Efekt Billboard - zawsze przodem do kamery
        if (billboardEffect && _mainCameraTransform != null && emojiRenderer.gameObject.activeSelf)
        {
            emojiRenderer.transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward,
                _mainCameraTransform.rotation * Vector3.up);
        }
    }

    public void UpdateEmoji(string actionName)
    {
        // Jeœli nazwa jest pusta lub to "Idle", mo¿emy ukryæ emoji (zale¿y od preferencji)
        if (string.IsNullOrEmpty(actionName) || actionName.Contains("Idle"))
        {
            HideEmoji();
            return;
        }

        Sprite icon = iconDatabase.GetIcon(actionName);

        if (icon != null)
        {
            emojiRenderer.sprite = icon;
            ShowEmoji();
        }
        else
        {
            HideEmoji();
        }
    }

    private void ShowEmoji()
    {
        emojiRenderer.gameObject.SetActive(true);
        // Tu mo¿na dodaæ prost¹ animacjê skali, np.:
        // emojiRenderer.transform.localScale = Vector3.zero;
        // emojiRenderer.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    private void HideEmoji()
    {
        emojiRenderer.gameObject.SetActive(false);
    }
}