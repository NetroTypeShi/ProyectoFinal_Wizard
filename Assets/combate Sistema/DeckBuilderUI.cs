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
    public int maxDeckSize = 5;

    [Header("Panel principal del deckbuilder")]
    public GameObject deckBuilderPanel;

    private DeckManager deckManager;

    private void Start()
    {
        deckManager = DeckManager.instance;

        if (deckManager == null)
        {
            Debug.LogError("❌ DeckManager no existe.");
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

        // ⭐ 1. CARTAS BASE DEL JUGADOR
        foreach (var card in cardCollection.unlockedCards)
            CrearCartaEnInventario(card);

        // ⭐ 2. CARTAS DESBLOQUEADAS EN EL MUNDO
        foreach (var card in deckManager.cartasDesbloqueadas)
        {
            if (!cardCollection.unlockedCards.Contains(card))
                CrearCartaEnInventario(card);
        }

        // ⭐ 3. MAZO EDITABLE
        foreach (var card in deckManager.mazoEditable)
        {
            GameObject go = Instantiate(cartaVisualPrefab, deckPanel);
            AjustarRectTransform(go);

            CartaVisual visual = go.GetComponent<CartaVisual>();
            visual.isFaceDown = false;
            visual.Configurar(card);

            go.AddComponent<DeckBuilderCardButton>().Init(card, this, false);
        }
    }

    private void CrearCartaEnInventario(CardData card)
    {
        GameObject go = Instantiate(cartaVisualPrefab, inventoryPanel);
        AjustarRectTransform(go);

        CartaVisual visual = go.GetComponent<CartaVisual>();
        visual.isFaceDown = false;
        visual.Configurar(card);

        go.AddComponent<DeckBuilderCardButton>().Init(card, this, true);
    }

    private void AjustarRectTransform(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();

        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        rt.anchoredPosition3D = new Vector3(rt.anchoredPosition.x, rt.anchoredPosition.y, 0);
    }

    public void AddCard(CardData card)
    {
        if (deckManager.mazoEditable.Count >= maxDeckSize)
        {
            Debug.Log("❌ Mazo lleno");
            return;
        }

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





