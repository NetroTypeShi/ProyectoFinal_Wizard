using UnityEngine;
using TMPro;
using System.Collections;

public class LevelTextAnimation : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public float fadeDuration = 0.6f;
    public float popScale = 1.2f;

    private void OnEnable()
    {
        StartCoroutine(AnimateLevel());
    }

    private IEnumerator AnimateLevel()
    {
        // Reset
        levelText.alpha = 0;
        transform.localScale = Vector3.one * 0.8f;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;

            // Fade in
            levelText.alpha = t;

            // Pop scale
            float scale = Mathf.Lerp(0.8f, popScale, Mathf.Sin(t * Mathf.PI));
            transform.localScale = new Vector3(scale, scale, 1);

            yield return null;
        }

        // Volver a escala normal
        transform.localScale = Vector3.one;
        levelText.alpha = 1;
    }
}

