using UnityEngine;
using UnityEngine.UI;

public class DeckBuilderCardButton : MonoBehaviour
{
    private CardData card;
    private DeckBuilderUI builder;
    private bool isInventoryCard;

    public void Init(CardData card, DeckBuilderUI builder, bool isInventoryCard)
    {
        this.card = card;
        this.builder = builder;
        this.isInventoryCard = isInventoryCard;

        // Añadir botón automáticamente
        Button btn = gameObject.AddComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (isInventoryCard)
            builder.AddCard(card);
        else
            builder.RemoveCard(card);
    }
}

