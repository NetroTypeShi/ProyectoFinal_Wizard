using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;

    [Header("Mazo editable por el jugador (deckbuilder)")]
    public List<CardData> mazoEditable = new List<CardData>();

    [Header("Mazo usado en combate (se genera desde mazoEditable)")]
    public List<CardData> mazoInicial = new List<CardData>();

    private List<CardData> mazo = new List<CardData>();
    private List<CardData> descarte = new List<CardData>();
    private List<CardData> mano = new List<CardData>();

    public int tamañoMano = 5;

    private void Awake()
    {
        // Evitar duplicados del singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Crear el mazo inicial desde el editable
        ActualizarMazoInicial();

        // Preparar el mazo para el combate
        ReiniciarMazo();
    }

    public void ActualizarMazoInicial()
    {
        // Copiar el mazo editable al mazo inicial
        mazoInicial = new List<CardData>(mazoEditable);
    }

    public void ReiniciarMazo()
    {
        mazo.Clear();
        descarte.Clear();
        mano.Clear();

        // Cargar el mazo inicial
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
        // Si el mazo está vacío, reciclar descarte
        if (mazo.Count == 0 && descarte.Count > 0)
        {
            mazo.AddRange(descarte);
            descarte.Clear();
            Barajar(mazo);
        }

        // Robar solo si hay espacio en la mano
        if (mazo.Count > 0 && mano.Count < tamañoMano)
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

    // Funciones para el deckbuilder
    public bool AñadirAlMazo(CardData carta, int maxSize)
    {
        if (mazoEditable.Count >= maxSize)
            return false;

        mazoEditable.Add(carta);
        return true;
    }

    public void QuitarDelMazo(CardData carta)
    {
        mazoEditable.Remove(carta);
    }
}





