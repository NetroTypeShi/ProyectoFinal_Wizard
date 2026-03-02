using UnityEngine;

public class DeckBuilderUI : MonoBehaviour
{
    public PlayerCardCollection cardCollection;

    [Header("Paneles donde se instancian las cartas")]
    public RectTransform inventoryPanel;
    public RectTransform deckPanel;

    [Header("Prefab de carta (EL MISMO QUE EN COMBATE)")]
    public GameObject cartaVisualPrefab;

    [Header("Límites")]
    public int maxDeckSize = 10;

    [Header("Panel principal del deckbuilder")]
    public GameObject deckBuilderPanel;

    private DeckManager deckManager;

    private void Start()
    {
        deckManager = DeckManager.instance;

        if (deckManager == null)
        {
            Debug.LogError("❌ DeckManager no existe. Asegúrate de cargar la escena inicial primero.");
            return;
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        // Limpiar paneles
        foreach (Transform t in inventoryPanel)
            Destroy(t.gameObject);

        foreach (Transform t in deckPanel)
            Destroy(t.gameObject);

        // Instanciar cartas del inventario (todas las desbloqueadas)
        foreach (var card in cardCollection.unlockedCards)
        {
            GameObject go = Instantiate(cartaVisualPrefab, inventoryPanel);
            AjustarRectTransform(go);

            CartaVisual visual = go.GetComponent<CartaVisual>();
            visual.isFaceDown = false;
            visual.Configurar(card);

            // Añadir lógica de añadir al mazo
            go.AddComponent<DeckBuilderCardButton>().Init(card, this, true);
        }

        // Instanciar cartas del mazo editable
        foreach (var card in deckManager.mazoEditable)
        {
            GameObject go = Instantiate(cartaVisualPrefab, deckPanel);
            AjustarRectTransform(go);

            CartaVisual visual = go.GetComponent<CartaVisual>();
            visual.isFaceDown = false;
            visual.Configurar(card);

            // Añadir lógica de quitar del mazo
            go.AddComponent<DeckBuilderCardButton>().Init(card, this, false);
        }
    }

    // 🔥 Ajusta la carta para que SIEMPRE se vea en UI
    private void AjustarRectTransform(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();

        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;

        // MUY IMPORTANTE: Z = 0 para que se renderice en UI
        rt.anchoredPosition3D = new Vector3(rt.anchoredPosition.x, rt.anchoredPosition.y, 0);
    }

    public void AddCard(CardData card)
    {
        if (deckManager.mazoEditable.Count >= maxDeckSize)
            return;

        deckManager.mazoEditable.Add(card);
        RefreshUI();
    }

    public void RemoveCard(CardData card)
    {
        deckManager.mazoEditable.Remove(card);
        RefreshUI();
    }

    public void CerrarDeckbuilder()
    {
        deckBuilderPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}




