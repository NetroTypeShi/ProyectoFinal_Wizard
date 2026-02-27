using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyCardAnimator : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI cardName;
    public CanvasGroup canvasGroup;

    [Header("Escala")]
    public float startScale = 0.2f;
    public float endScale = 1f;

    [Header("Duraciones")]
    public float fadeDuration = 0.15f;
    public float moveDuration = 0.45f;

    public IEnumerator PlayCardAnimation(Sprite sprite, string name, Vector3 startPos, Vector3 endPos)
    {
        if (cardImage == null || cardName == null || canvasGroup == null)
            yield break;

        cardImage.sprite = sprite;
        cardName.text = name;

        transform.position = startPos;
        transform.localScale = Vector3.one * startScale;
        canvasGroup.alpha = 0f;

        Vector3 controlPoint = (startPos + endPos) / 2f;
        controlPoint.x = startPos.x - 120f;
        controlPoint.y = Mathf.Lerp(startPos.y, endPos.y, 0.5f);

        // FADE IN
        float t = 0;
        while (t < fadeDuration)
        {
            if (canvasGroup == null) yield break;

            t += Time.deltaTime;
            float p = t / fadeDuration;
            canvasGroup.alpha = p;
            yield return null;
        }

        // MOVIMIENTO EN CURVA
        t = 0;
        while (t < moveDuration)
        {
            if (canvasGroup == null || transform == null) yield break;

            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / moveDuration);

            Vector3 pos =
                Mathf.Pow(1 - p, 2) * startPos +
                2 * (1 - p) * p * controlPoint +
                Mathf.Pow(p, 2) * endPos;

            transform.position = pos;

            float scale = Mathf.Lerp(startScale, endScale, p);
            transform.localScale = Vector3.one * scale;

            yield return null;
        }

        yield return new WaitForSeconds(0.4f);

        // FADE OUT
        t = 0;
        while (t < fadeDuration)
        {
            if (canvasGroup == null || transform == null) yield break;

            t += Time.deltaTime;
            float p = t / fadeDuration;

            canvasGroup.alpha = 1f - p;
            transform.localScale = Vector3.Lerp(Vector3.one * endScale, Vector3.zero, p);

            yield return null;
        }

        Destroy(gameObject, 0.05f);
    }
}




