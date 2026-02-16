using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarUI : MonoBehaviour
{
    [Header("Tag del personaje que esta barra representa")]
    public string targetTag; // "Player" o "Enemy"

    [Header("Imagen del relleno")]
    public Image fillImage;

    [Header("Shake Settings")]
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 10f;

    private HealthComponent targetHealth;
    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        GameEvents.OnLifeChanged.AddListener(OnLifeEvent);
    }

    private void OnDisable()
    {
        GameEvents.OnLifeChanged.RemoveListener(OnLifeEvent);
    }

    private void OnLifeEvent(HealthComponent healthComp)
    {
        // Si aún no tenemos target, lo buscamos por tag
        if (targetHealth == null)
        {
            if (healthComp.CompareTag(targetTag))
                targetHealth = healthComp;
            else
                return;
        }

        // Si el evento no es del target, ignoramos
        if (healthComp != targetHealth)
            return;

        // Actualizar barra
        float amount = Mathf.Clamp01(
            (float)healthComp.currentHealth / healthComp.maxHealth
        );
        fillImage.fillAmount = amount;

        // Si ha recibido daño → shake
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            rectTransform.anchoredPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
    }
}



