using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ExperienceBar : MonoBehaviour
{
    [Header("UI")]
    public Image fillImage;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;

    [Header("Animación")]
    public float fillSpeed = 0.5f; // velocidad de animación

    private int lastLevel = -1;
    private bool animating = false;

    private void OnEnable()
    {
        // Cuando se activa (en la pantalla de victoria), animamos la barra
        StartCoroutine(AnimateExp());
    }

    private IEnumerator AnimateExp()
    {
        animating = true;

        var exp = PlayerExperience.Instance;

        // Si es la primera vez, guardamos el nivel
        if (lastLevel == -1)
            lastLevel = exp.level;

        // Actualizamos textos
        levelText.text = "Nv. " + exp.level;
        expText.text = exp.currentExp + " / " + exp.expToNextLevel;

        // Valor actual de la barra
        float startFill = fillImage.fillAmount;
        float targetFill = (float)exp.currentExp / exp.expToNextLevel;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            fillImage.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        fillImage.fillAmount = targetFill;

        // ⭐ Si subió de nivel, animamos un destello
        if (exp.level > lastLevel)
        {
            StartCoroutine(LevelUpFlash());
            lastLevel = exp.level;
        }

        animating = false;
    }

    private IEnumerator LevelUpFlash()
    {
        Color original = fillImage.color;

        // Destello dorado
        fillImage.color = new Color(1f, 0.9f, 0.4f);

        yield return new WaitForSeconds(0.2f);

        fillImage.color = original;
    }
}


