using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<CardData> mazoInicial;
    private List<CardData> mazo = new List<CardData>();
    private List<CardData> descarte = new List<CardData>();
    private List<CardData> mano = new List<CardData>();

    public int tamañoMano = 5;

    private void Awake()
    {
        ReiniciarMazo();
    }

    public void ReiniciarMazo()
    {
        mazo.Clear();
        descarte.Clear();
        mano.Clear();

        foreach (var c in mazoInicial)
            mazo.Add(c);

        Barajar(mazo);
        RobarManoInicial();
    }

    private void Barajar(List<CardData> lista)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            int rnd = Random.Range(i, lista.Count);
            var temp = lista[i];
            lista[i] = lista[rnd];
            lista[rnd] = temp;
        }
    }

    private void RobarManoInicial()
    {
        for (int i = 0; i < tamañoMano; i++)
            RobarCarta();
    }

    public void RobarCarta()
    {
        if (mazo.Count == 0 && descarte.Count > 0)
        {
            mazo.AddRange(descarte);
            descarte.Clear();
            Barajar(mazo);
        }

        if (mazo.Count > 0 && mano.Count < tamañoMano) // ← IMPORTANTE
        {
            var carta = mazo[0];
            mazo.RemoveAt(0);
            mano.Add(carta);
        }
    }

    public List<CardData> ObtenerMano()
    {
        return mano;
    }

    public void UsarCarta(int index)
    {
        var carta = mano[index];
        mano.RemoveAt(index);
        descarte.Add(carta);

        RobarCarta();
    }
}


