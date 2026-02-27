using UnityEngine;
using TMPro;
using System.Collections;

public class VictoryTextAnimation : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float fadeDuration = 1f;
    public float scaleDuration = 0.8f;
    public float startScale = 0.6f;
    public float endScale = 1f;
    public float moveUpAmount = 20f;

    private void OnEnable()
    {
        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        // Reset
        text.alpha = 0;
        transform.localScale = Vector3.one * startScale;

        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, moveUpAmount, 0);

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;

            // Fade in
            text.alpha = t;

            // Scale
            float scaleT = Mathf.SmoothStep(startScale, endScale, t);
            transform.localScale = new Vector3(scaleT, scaleT, 1);

            // Move up
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        text.alpha = 1;
        transform.localScale = Vector3.one * endScale;
        transform.localPosition = endPos;
    }
}


