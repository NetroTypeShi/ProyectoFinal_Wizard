using UnityEngine;
using UnityEngine.UI;

public class DeckBuilderCardButton : MonoBehaviour
{
    private CardData card;
    private DeckBuilderUI builder;
    private bool isInventoryCard;

    private Image img;
    private Shadow shadow;

    private Vector3 normalScale;
    private Vector3 selectedScale;

    public bool isSelected;

    private void Awake()
    {
        img = GetComponent<Image>();

        // Crear sombra si no existe
        shadow = GetComponent<Shadow>();
        if (shadow == null)
            shadow = gameObject.AddComponent<Shadow>();

        shadow.effectColor = new Color(0, 0, 0, 0.35f);
        shadow.effectDistance = new Vector2(4, -4);

        // Escalas
        normalScale = transform.localScale;
        selectedScale = normalScale * 1.12f; // 12% más grande al seleccionar

        // Fondo suave
        img.color = new Color(1f, 1f, 1f, 0.92f);
    }

    public void Init(CardData card, DeckBuilderUI builder, bool isInventoryCard)
    {
        this.card = card;
        this.builder = builder;
        this.isInventoryCard = isInventoryCard;

        Button btn = gameObject.AddComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (isInventoryCard)
            builder.AddCard(card);
        else
            builder.RemoveCard(card);
    }

    public void Select()
    {
        isSelected = true;

        // Borde brillante
        img.color = new Color(1f, 0.95f, 0.65f, 1f);

        // Sombra más fuerte
        shadow.effectColor = new Color(0, 0, 0, 0.55f);
        shadow.effectDistance = new Vector2(6, -6);

        // Zoom
        transform.localScale = selectedScale;
    }

    public void Deselect()
    {
        isSelected = false;

        // Fondo normal
        img.color = new Color(1f, 1f, 1f, 0.92f);

        // Sombra normal
        shadow.effectColor = new Color(0, 0, 0, 0.35f);
        shadow.effectDistance = new Vector2(4, -4);

        // Escala normal
        transform.localScale = normalScale;
    }
}



