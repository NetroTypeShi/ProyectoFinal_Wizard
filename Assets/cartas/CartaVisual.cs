using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CartaVisual : MonoBehaviour
{
    [Header("UI Elements")]
    public Image icono;
    public TMP_Text nameText;
    public TMP_Text manaText;
    public TMP_Text damageText;
    public TMP_Text healText;
    public TMP_Text defenseText;

    [Header("Lógica")]
    public CartaAtaque cartaLogic;

    [Header("Dorso")]
    public Sprite backSprite;     // ⭐ Aquí arrastras tu imagen normal (importada como Sprite)
    public bool isFaceDown = false;

    public void Configurar(CardData data)
    {
        // Guardar la lógica
        cartaLogic.data = data;

        if (isFaceDown)
        {
            // ⭐ Mostrar dorso
            icono.sprite = backSprite;
            nameText.text = "";
            manaText.text = "";
            damageText.gameObject.SetActive(false);
            healText.gameObject.SetActive(false);
            defenseText.gameObject.SetActive(false);
            return;
        }

        // ⭐ Mostrar cara normal
        icono.sprite = data.artwork;
        nameText.text = data.cardName;
        manaText.text = data.manaCost.ToString();

        // Ocultar todo por defecto
        damageText.gameObject.SetActive(false);
        healText.gameObject.SetActive(false);
        defenseText.gameObject.SetActive(false);

        // ⭐ Mostrar solo lo que corresponde según CardType
        switch (data.type)
        {
            case CardType.Damage:
                damageText.gameObject.SetActive(true);
                damageText.text = data.damage.ToString();
                break;

            case CardType.Heal:
                healText.gameObject.SetActive(true);
                healText.text = "+" + data.healAmount.ToString();
                break;

            case CardType.Defense:
                defenseText.gameObject.SetActive(true);
                defenseText.text = (data.defensePercent * 100f).ToString("0") + "%";
                break;
        }
    }
}







