using UnityEngine;
using System.Collections.Generic;

public class EnemyDeckManager : MonoBehaviour
{
    public EnemyStats enemyStats; // ⭐ referencia al ScriptableObject

    public int manoSize = 3;

    private List<CardData> cartasEnMazo = new List<CardData>();
    private List<CardData> manoActual = new List<CardData>();

    private void Start()
    {
        // Copiar el mazo base del ScriptableObject
        cartasEnMazo = new List<CardData>(enemyStats.mazoBase);

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
            return;

        int index = Random.Range(0, cartasEnMazo.Count);
        manoActual.Add(cartasEnMazo[index]);
        cartasEnMazo.RemoveAt(index);
    }

    public List<CardData> ObtenerMano()
    {
        return manoActual;
    }

    public void UsarCarta(int index)
    {
        if (index < 0 || index >= manoActual.Count)
            return;

        manoActual.RemoveAt(index);

        if (cartasEnMazo.Count > 0)
            RobarCarta();
    }
}

