using UnityEngine;
using System.Collections.Generic;

public class EnemyDeckManager : MonoBehaviour
{
    public List<CardData> cartasEnMazo;
    public int manoSize = 3;

    private List<CardData> manoActual = new List<CardData>();

    private void Start()
    {
        RobarManoInicial();
    }

    private void RobarManoInicial()
    {
        manoActual.Clear();

        for (int i = 0; i < manoSize; i++)
            RobarCarta();
    }

    private void RobarCarta()
    {
        if (cartasEnMazo.Count == 0)
        {
            Debug.LogWarning("El mazo del enemigo está vacío");
            return;
        }

        int index = Random.Range(0, cartasEnMazo.Count);
        manoActual.Add(cartasEnMazo[index]);
        cartasEnMazo.RemoveAt(index); // ← IMPORTANTE
    }

    public List<CardData> ObtenerMano()
    {
        return manoActual;
    }

    public void UsarCarta(int index)
    {
        manoActual.RemoveAt(index);
        RobarCarta();
    }
}



