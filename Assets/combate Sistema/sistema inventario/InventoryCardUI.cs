using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryCardUI : MonoBehaviour
{
    public TMP_Text cardName;
    private CardData card;
    private DeckBuilderUI deckUI;

    public void Setup(CardData card, DeckBuilderUI ui)
    {
        this.card = card;
        this.deckUI = ui;
        cardName.text = card.cardName;
    }

    public void OnAdd()
    {
        deckUI.AddCard(card);
    }
}
