using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CartaVisual : MonoBehaviour
{
    [Header("UI Elements")]
    public Image icono;                     // Imagen principal de la carta


    [Header("Lógica")]
    public CartaAtaque cartaLogic;

    public void Configurar(CardData data)
    {
        // Asignar datos a la lógica
        cartaLogic.data = data;

        // Asignar imagen
        icono.sprite = data.artwork;

       
    }
}




