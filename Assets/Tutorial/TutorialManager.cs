using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject tutorialPanel;
    public Image tutorialImage;
    public Button continuarBtn;

    [Header("Sprites - en orden")]
    [Tooltip("0,1,2: intro | 3: vida baja | 4: fin tutorial")]
    public Sprite[] tutorialSprites;

    [Header("Referencias")]
    public CombatController combat;

    public int paso = 0;
    public bool tutorialActivo = true;
    private bool puedeContinuar = true;

    private void Start()
    {
        continuarBtn.onClick.AddListener(Continuar);
        StartCoroutine(IniciarTutorial());
    }

    private IEnumerator IniciarTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        combat.BloquearCartas();
        MostrarImagen(0);
    }

    public void Continuar()
    {
        if (!puedeContinuar) return;
        StartCoroutine(ContinuarConCooldown());
    }

    private IEnumerator ContinuarConCooldown()
    {
        puedeContinuar = false;
        continuarBtn.interactable = false;
        EventSystem.current.SetSelectedGameObject(null);

        paso++;

        switch (paso)
        {
            case 1:
                // Segunda imagen
                MostrarImagen(1);
                break;

            case 2:
                // Tercera imagen
                MostrarImagen(2);
                break;

            case 3:
                // Ocultar panel, esperar carta ataque
                OcultarPanel();
                combat.SetCartaPermitida("ataque1");
                combat.DesbloquearCartas();
                combat.esperandoSeleccion = true;
                break;

            case 5:
                // Ocultar panel vida baja, esperar carta curación
                OcultarPanel();
                combat.SetCartaPermitida("vida");
                combat.DesbloquearCartas();
                combat.esperandoSeleccion = true;
                break;

            case 7:
                // Ocultar imagen final, combate libre
                OcultarPanel();
                combat.SetCartaPermitida(null);
                combat.DesbloquearCartas();
                combat.esperandoSeleccion = true;
                tutorialActivo = false;
                break;
        }

        yield return new WaitForSeconds(0.3f);
        puedeContinuar = true;
        continuarBtn.interactable = true;
    }

    public void OnCartaUsada(CardData carta)
    {
        if (!tutorialActivo) return;

        if (paso == 3 && carta.cardName == "ataque1")
        {
            combat.SetCartaPermitida(null);
            combat.BloquearCartas();
        }

        if (paso == 5 && carta.cardName == "vida")
        {
            combat.SetCartaPermitida(null);
            combat.BloquearCartas();
            paso = 6;
            StartCoroutine(MostrarImagenConRetraso(4, 2f));
        }
    }

    public void OnEnemigoAtaco()
    {
        if (!tutorialActivo) return;
        if (paso != 3) return;

        paso = 4;
        combat.BloquearCartas();
        MostrarImagen(3);
    }

    private IEnumerator MostrarImagenConRetraso(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        MostrarImagen(index);
    }

    private void MostrarImagen(int index)
    {
        if (index >= tutorialSprites.Length) return;
        tutorialPanel.SetActive(true);
        tutorialImage.sprite = tutorialSprites[index];
        continuarBtn.gameObject.SetActive(true);
    }

    private void OcultarPanel()
    {
        tutorialPanel.SetActive(false);
    }
}