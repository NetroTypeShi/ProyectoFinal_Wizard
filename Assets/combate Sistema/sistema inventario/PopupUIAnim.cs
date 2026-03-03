using UnityEngine;
using TMPro;

public class PopupUIAnim : MonoBehaviour
{
    public float duration = 1.5f;
    public float moveUpDistance = 40f;

    private TMP_Text text;
    private Color originalColor;
    private RectTransform rect;
    private Vector2 startPos;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        rect = GetComponent<RectTransform>();

        originalColor = text.color;
        startPos = rect.anchoredPosition;
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(AnimationRoutine());
    }

    private System.Collections.IEnumerator AnimationRoutine()
    {
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            // Subir
            rect.anchoredPosition = startPos + Vector2.up * (moveUpDistance * progress);

            // Fade out
            Color c = originalColor;
            c.a = 1f - progress;
            text.color = c;

            yield return null;
        }

        // Reset
        rect.anchoredPosition = startPos;
        text.color = originalColor;

        // Ocultar
        gameObject.SetActive(false);
    }
}

