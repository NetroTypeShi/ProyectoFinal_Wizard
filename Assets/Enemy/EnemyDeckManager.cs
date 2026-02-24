using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDeckManager : MonoBehaviour
{
    public EnemyStats enemyStats;

    public int manoSize = 5;

    private List<CardData> mazo = new List<CardData>();
    private List<CardData> descarte = new List<CardData>();
    private List<CardData> mano = new List<CardData>();

    [Header("Visual mano enemigo")]
    public GameObject enemyCardPrefab;
    public Transform enemyHandSpawnPoint;
    public float separacion = 120f;

    [Header("Escala de cartas en la mano")]
    public float handCardScale = 0.6f;

    public Canvas canvas;

    [Header("Animación carta jugada")]
    public Transform cardCenterPoint;

    private List<GameObject> cartasInstanciadas = new List<GameObject>();

    private void Start()
    {
        ReiniciarMazo();
    }

    public void ReiniciarMazo()
    {
        mazo.Clear();
        descarte.Clear();
        mano.Clear();

        foreach (var c in enemyStats.mazoBase)
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
        for (int i = 0; i < manoSize; i++)
            RobarCarta();

        MostrarManoEnemigo();
    }

    public void RobarCarta()
    {
        if (mazo.Count == 0 && descarte.Count > 0)
        {
            mazo.AddRange(descarte);
            descarte.Clear();
            Barajar(mazo);
        }

        if (mazo.Count > 0 && mano.Count < manoSize)
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
        if (index < 0 || index >= mano.Count)
            return;

        CardData carta = mano[index];

        StartCoroutine(AnimarCartaReal(index, carta));
    }

    private IEnumerator AnimarCartaReal(int index, CardData carta)
    {
        GameObject cartaGO = cartasInstanciadas[index];

        // Revelar carta
        CartaVisual visual = cartaGO.GetComponent<CartaVisual>();
        visual.isFaceDown = false;
        visual.Configurar(carta);

        EnemyCardAnimator anim = cartaGO.GetComponent<EnemyCardAnimator>();
        anim.startScale = handCardScale;

        // Animación
        yield return StartCoroutine(anim.PlayCardAnimation(
            carta.artwork,
            carta.cardName,
            cartaGO.transform.position,
            cardCenterPoint.position
        ));

        // Destruir carta visual
        Destroy(cartaGO);
        cartasInstanciadas.RemoveAt(index);

        // Mover carta a descarte
        mano.RemoveAt(index);
        descarte.Add(carta);

        // Robar nueva carta
        RobarCarta();

        // Actualizar mano visual
        MostrarManoEnemigo();
    }

    public void MostrarManoEnemigo()
    {
        foreach (var c in cartasInstanciadas)
            Destroy(c);

        cartasInstanciadas.Clear();

        for (int i = 0; i < mano.Count; i++)
        {
            GameObject cartaGO = Instantiate(enemyCardPrefab);
            cartaGO.transform.SetParent(enemyHandSpawnPoint, false);

            RectTransform rt = cartaGO.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * separacion, 0);
            rt.localScale = Vector3.one * handCardScale;

            CartaVisual visual = cartaGO.GetComponent<CartaVisual>();
            visual.isFaceDown = true;
            visual.Configurar(mano[i]);

            cartasInstanciadas.Add(cartaGO);
        }
    }
}





