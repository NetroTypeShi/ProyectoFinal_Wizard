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
    public float startScale = 0.2f;   // ⭐ Se sobrescribe desde EnemyDeckManager
    public float endScale = 1f;

    [Header("Duraciones")]
    public float fadeDuration = 0.15f;
    public float moveDuration = 0.45f;

    public IEnumerator PlayCardAnimation(Sprite sprite, string name, Vector3 startPos, Vector3 endPos)
    {
        cardImage.sprite = sprite;
        cardName.text = name;

        transform.position = startPos;
        transform.localScale = Vector3.one * startScale;
        canvasGroup.alpha = 0f;

        // ⭐ Punto de control para curva izquierda → derecha
        Vector3 controlPoint = (startPos + endPos) / 2f;

        controlPoint.x = startPos.x - 120f;
        controlPoint.y = Mathf.Lerp(startPos.y, endPos.y, 0.5f);

        // ⭐ FADE IN
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = t / fadeDuration;
            canvasGroup.alpha = p;
            yield return null;
        }

        // ⭐ MOVIMIENTO EN CURVA (Bezier)
        t = 0;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / moveDuration);

            Vector3 pos =
                Mathf.Pow(1 - p, 2) * startPos +
                2 * (1 - p) * p * controlPoint +
                Mathf.Pow(p, 2) * endPos;

            transform.position = pos;

            // ⭐ Escala dinámica
            float scale = Mathf.Lerp(startScale, endScale, p);
            transform.localScale = Vector3.one * scale;

            yield return null;
        }

        // ⭐ Espera un momento
        yield return new WaitForSeconds(0.4f);

        // ⭐ FADE OUT + SCALE OUT
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = t / fadeDuration;

            canvasGroup.alpha = 1f - p;
            transform.localScale = Vector3.Lerp(Vector3.one * endScale, Vector3.zero, p);

            yield return null;
        }

        Destroy(gameObject);
    }
}



