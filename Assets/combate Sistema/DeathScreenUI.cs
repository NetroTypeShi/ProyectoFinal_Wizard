using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DeathScreenUI : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI deathText;

    public float fadeDuration = 1.5f;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ShowDeathScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;

        Color bgColor = background.color;
        Color textColor = deathText.color;

        bgColor.a = 0f;
        textColor.a = 0f;

        background.color = bgColor;
        deathText.color = textColor;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);

            bgColor.a = alpha * 0.85f; 
            textColor.a = alpha;

            background.color = bgColor;
            deathText.color = textColor;

            yield return null;
        }
    }
}
