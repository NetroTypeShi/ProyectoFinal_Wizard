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

    public void Configurar(CardData data)
    {
        cartaLogic.data = data;

        // Imagen
        icono.sprite = data.artwork;

        // Nombre
        nameText.text = data.cardName;

        // Coste de maná
        manaText.text = data.manaCost.ToString();

        // Ocultar todo por defecto
        damageText.gameObject.SetActive(false);
        healText.gameObject.SetActive(false);
        defenseText.gameObject.SetActive(false);

        // Mostrar solo lo que corresponde
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





