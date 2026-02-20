using UnityEngine;
using System.Collections;

public class SceneIntroTransition : MonoBehaviour
{
    public CanvasGroup panelTransicion;
    public float duracionFade = 3.0f;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;

        // Asegurar que empieza totalmente negro
        panelTransicion.alpha = 1f;

        while (t < duracionFade)
        {
            t += Time.deltaTime;
            float p = t / duracionFade;

            panelTransicion.alpha = Mathf.Lerp(1f, 0f, p);

            yield return null;
        }

        panelTransicion.gameObject.SetActive(false);
    }
}

