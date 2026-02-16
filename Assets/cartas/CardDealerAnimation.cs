using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDealerAnimation : MonoBehaviour
{
    [Header("Punto inicial (fuera de la pantalla a la derecha)")]
    public RectTransform spawnPoint;

    [Header("Duración del movimiento de cada carta")]
    public float moveDuration = 0.5f;

    [Header("Retraso entre carta y carta")]
    public float dealDelay = 0.1f;

    public void DealCards(List<RectTransform> cards, List<RectTransform> finalPositions)
    {
        StartCoroutine(DealRoutine(cards, finalPositions));
    }

    private IEnumerator DealRoutine(List<RectTransform> cards, List<RectTransform> finalPositions)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform card = cards[i];
            RectTransform target = finalPositions[i];

            // Colocar la carta en el punto inicial (derecha)
            card.anchoredPosition = spawnPoint.anchoredPosition;

            // Animar hacia su posición final
            StartCoroutine(MoveCard(card, target.anchoredPosition));

            // Delay entre cartas
            yield return new WaitForSeconds(dealDelay);
        }
    }

    private IEnumerator MoveCard(RectTransform card, Vector2 targetPos)
    {
        Vector2 startPos = card.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            t = Mathf.SmoothStep(0, 1, t);

            card.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        card.anchoredPosition = targetPos;
    }
}

