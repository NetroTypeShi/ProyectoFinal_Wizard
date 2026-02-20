using UnityEngine;
using System.Collections.Generic;

public class EnemyDeckManager : MonoBehaviour
{
    public EnemyStats enemyStats;

    public int manoSize = 3;

    private List<CardData> cartasEnMazo = new List<CardData>();
    private List<CardData> manoActual = new List<CardData>();

    [Header("Animación de cartas")]
    public GameObject enemyCardPrefab;
    public Transform enemyCardSpawnPoint;
    public Transform cardCenterPoint;
    public Canvas canvas;

    [Header("Efecto Hearthstone")]
    public float fanRadius = 150f;   // distancia del abanico
    public float fanAngle = 40f;     // apertura del abanico

    private void Start()
    {
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

        CardData carta = manoActual[index];

        // ⭐ Mostrar animación estilo Hearthstone
        MostrarCartaAnimada(carta);

        manoActual.RemoveAt(index);

        if (cartasEnMazo.Count > 0)
            RobarCarta();
    }

    private Vector3 GetRandomFanPosition()
    {
        float angle = Random.Range(-fanAngle, fanAngle);
        Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.up;
        return enemyCardSpawnPoint.position + dir * fanRadius;
    }

    private void MostrarCartaAnimada(CardData carta)
    {
        GameObject obj = Instantiate(enemyCardPrefab, canvas.transform);

        EnemyCardAnimator anim = obj.GetComponent<EnemyCardAnimator>();

        StartCoroutine(anim.PlayCardAnimation(
            carta.artwork,
            carta.cardName,
            GetRandomFanPosition(),        // ⭐ ahora sale desde un abanico
            cardCenterPoint.position
        ));
    }
}



