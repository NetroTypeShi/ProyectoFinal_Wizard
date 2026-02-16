using UnityEngine;
using UnityEngine.UI;

public class ManaOrbUI : MonoBehaviour
{
    [Header("Referencia al relleno del orbe")]
    public Image fillImage;

    private void OnEnable()
    {
        GameEvents.OnManaChanged.AddListener(UpdateMana);
    }

    private void OnDisable()
    {
        GameEvents.OnManaChanged.RemoveListener(UpdateMana);
    }

    private void UpdateMana(int manaActual, int manaMax)
    {
        if (fillImage == null)
        {
            Debug.LogError("ManaOrbUI: No se asignó fillImage.");
            return;
        }

        float amount = Mathf.Clamp01((float)manaActual / manaMax);
        fillImage.fillAmount = amount;
    }
}




