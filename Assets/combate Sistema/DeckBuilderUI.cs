using UnityEngine;
using System.Collections.Generic;

public class DeckBuilderUI : MonoBehaviour
{
    public PlayerCardCollection cardCollection;

    public RectTransform inventoryPanel;
    public RectTransform deckPanel;

    public GameObject cartaVisualPrefab;

    public int maxDeckSize = 5;
    public GameObject deckBuilderPanel;

    private DeckManager deckManager;

    private List<DeckBuilderCardButton> inventoryButtons = new List<DeckBuilderCardButton>();
    private List<DeckBuilderCardButton> deckButtons = new List<DeckBuilderCardButton>();

    private int selectedInventoryIndex = 0;
    private int selectedDeckIndex = 0;

    private bool inInventory = true;

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

    // ✅ FIX: Registrar listener y refrescar cada vez que el panel se activa
    private void OnEnable()
    {
        // Evitar doble-registro si Start ya añadió el listener
        GameEvents.OnCardUnlocked.RemoveListener(OnCardUnlocked);
        GameEvents.OnCardUnlocked.AddListener(OnCardUnlocked);

        // Si deckManager ya está listo (Start ya corrió), refrescar
        if (deckManager != null)
            RefreshUI();
    }

    // ✅ FIX: Desregistrar listener cuando el panel se desactiva
    private void OnDisable()
    {
        GameEvents.OnCardUnlocked.RemoveListener(OnCardUnlocked);
    }

    private void OnCardUnlocked(CardData card)
    {
        // ✅ Solo refrescar si el panel está visible
        if (gameObject.activeInHierarchy)
            RefreshUI();
    }

    private void Update()
    {
        // ❗ FIX: No bloquear Update si no hay cartas
        if (inInventory && inventoryButtons.Count == 0)
            return;

        if (!inInventory && deckButtons.Count == 0)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
            SwitchPanel();

        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelection(1);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelection(-1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveSelection(4);
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveSelection(-4);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inInventory)
                inventoryButtons[selectedInventoryIndex].OnClick();
            else
                deckButtons[selectedDeckIndex].OnClick();
        }
    }

    private void SwitchPanel()
    {
        if (inInventory)
        {
            if (inventoryButtons.Count > 0)
                inventoryButtons[selectedInventoryIndex].Deselect();

            inInventory = false;

            if (deckButtons.Count > 0)
                deckButtons[selectedDeckIndex].Select();
        }
        else
        {
            if (deckButtons.Count > 0)
                deckButtons[selectedDeckIndex].Deselect();

            inInventory = true;

            if (inventoryButtons.Count > 0)
                inventoryButtons[selectedInventoryIndex].Select();
        }
    }

    private void MoveSelection(int amount)
    {
        if (inInventory)
        {
            inventoryButtons[selectedInventoryIndex].Deselect();

            selectedInventoryIndex += amount;
            selectedInventoryIndex = Mathf.Clamp(selectedInventoryIndex, 0, inventoryButtons.Count - 1);

            inventoryButtons[selectedInventoryIndex].Select();
        }
        else
        {
            deckButtons[selectedDeckIndex].Deselect();

            selectedDeckIndex += amount;
            selectedDeckIndex = Mathf.Clamp(selectedDeckIndex, 0, deckButtons.Count - 1);

            deckButtons[selectedDeckIndex].Select();
        }
    }

    public void RefreshUI()
    {
        inventoryButtons.Clear();
        deckButtons.Clear();
        selectedInventoryIndex = 0;
        selectedDeckIndex = 0;
        inInventory = true;

        foreach (Transform t in inventoryPanel)
            Destroy(t.gameObject);

        foreach (Transform t in deckPanel)
            Destroy(t.gameObject);

        foreach (var card in cardCollection.unlockedCards)
            CrearCartaEnInventario(card);

        foreach (var card in deckManager.cartasDesbloqueadas)
        {
            if (!cardCollection.unlockedCards.Contains(card))
                CrearCartaEnInventario(card);
        }

        foreach (var card in deckManager.mazoEditable)
        {
            GameObject go = Instantiate(cartaVisualPrefab, deckPanel);
            AjustarRectTransform(go);

            CartaVisual visual = go.GetComponent<CartaVisual>();
            visual.isFaceDown = false;
            visual.Configurar(card);

            var btn = go.AddComponent<DeckBuilderCardButton>();
            btn.Init(card, this, false);

            deckButtons.Add(btn);
        }

        // ❗ FIX: Evitar que Update se bloquee si no hay cartas
        if (inventoryButtons.Count > 0)
            inventoryButtons[0].Select();
        else
            inInventory = false;
    }

    private void CrearCartaEnInventario(CardData card)
    {
        GameObject go = Instantiate(cartaVisualPrefab, inventoryPanel);
        AjustarRectTransform(go);

        CartaVisual visual = go.GetComponent<CartaVisual>();
        visual.isFaceDown = false;
        visual.Configurar(card);

        var btn = go.AddComponent<DeckBuilderCardButton>();
        btn.Init(card, this, true);

        inventoryButtons.Add(btn);
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




