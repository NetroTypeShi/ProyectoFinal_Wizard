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

    private void OnEnable()
    {
        GameEvents.OnCardUnlocked.RemoveListener(OnCardUnlocked);
        GameEvents.OnCardUnlocked.AddListener(OnCardUnlocked);

        if (deckManager == null)
            deckManager = DeckManager.instance;

        if (deckManager != null)
            RefreshUI();
    }

    private void OnDisable()
    {
        GameEvents.OnCardUnlocked.RemoveListener(OnCardUnlocked);
    }

    private void OnCardUnlocked(CardData card)
    {
        if (gameObject.activeInHierarchy)
            RefreshUI();
    }

    private void Update()
    {
        // ABRIR / CERRAR CON TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (deckBuilderPanel.activeSelf)
                CerrarDeckbuilder();
            else
                AbrirDeckbuilder();

            return; 
        }

        
        if (!deckBuilderPanel.activeSelf)
            return;

        // --- Navegación  ---
        if (inInventory && inventoryButtons.Count == 0)
            return;

        if (!inInventory && deckButtons.Count == 0)
            return;

        if (Input.GetKeyDown(KeyCode.D)) MoveSelection(1);
        if (Input.GetKeyDown(KeyCode.A)) MoveSelection(-1);
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

            int nuevoIndex = selectedInventoryIndex + amount;

            // Si está en la última carta y pulsa derecha, cambia al mazo
            if (amount == 1 && selectedInventoryIndex == inventoryButtons.Count - 1 && deckButtons.Count > 0)
            {
                inInventory = false;
                deckButtons[selectedDeckIndex].Select();
                return;
            }

            // Movimiento normal dentro del inventario
            selectedInventoryIndex = Mathf.Clamp(nuevoIndex, 0, inventoryButtons.Count - 1);
            inventoryButtons[selectedInventoryIndex].Select();
        }
        else
        {
            deckButtons[selectedDeckIndex].Deselect();

            int nuevoIndex = selectedDeckIndex + amount;

            // Si está en la primera carta y pulsa izquierda, vuelve al inventario
            if (amount == -1 && selectedDeckIndex == 0 && inventoryButtons.Count > 0)
            {
                inInventory = true;
                inventoryButtons[selectedInventoryIndex].Select();
                return;
            }

            // Movimiento normal dentro del mazo
            selectedDeckIndex = Mathf.Clamp(nuevoIndex, 0, deckButtons.Count - 1);
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


    public void AbrirDeckbuilder()
    {
        deckBuilderPanel.SetActive(true);
        Time.timeScale = 0f; // Pausa el juego
        RefreshUI();
    }


}





