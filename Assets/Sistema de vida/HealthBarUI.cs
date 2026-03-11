using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthBarUI : MonoBehaviour
{
    [Header("Imagen del relleno")]
    public Image fillImage;

    [Header("Texto de vida")]
    public TextMeshProUGUI lifeText;

    [Header("Materiales de estado")]
    public Material normalMaterial;
    public Material paralizadoMaterial;

    private HealthComponent targetHealth;
    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        // Asegurar que arranca con el material normal
        if (fillImage != null && normalMaterial != null)
            fillImage.material = normalMaterial;
    }

    private void OnEnable()
    {
        GameEvents.OnLifeChanged.AddListener(OnLifeEvent);
    }

    private void OnDisable()
    {
        GameEvents.OnLifeChanged.RemoveListener(OnLifeEvent);
    }

    // ⭐ Se llama desde CombatController
    public void SetTarget(HealthComponent health)
    {
        targetHealth = health;

        // Forzar actualización inmediata
        OnLifeEvent(health);
    }

    private void OnLifeEvent(HealthComponent healthComp)
    {
        if (targetHealth == null) return;
        if (healthComp != targetHealth) return;

        float amount = Mathf.Clamp01((float)healthComp.currentHealth / healthComp.maxHealth);
        fillImage.fillAmount = amount;

        if (lifeText != null)
            lifeText.text = $"{healthComp.currentHealth} / {healthComp.maxHealth}";
    }

    // ⭐ Cambiar material cuando está paralizado
    public void SetParalizado(bool estado)
    {
        if (fillImage == null) return;

        if (estado)
        {
            if (paralizadoMaterial != null)
                fillImage.material = paralizadoMaterial;
        }
        else
        {
            if (normalMaterial != null)
                fillImage.material = normalMaterial;
        }
    }
}
