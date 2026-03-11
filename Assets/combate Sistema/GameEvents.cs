using UnityEngine;
using UnityEngine.Events;

public static class GameEvents
{
    public static UnityEvent onRestart = new UnityEvent();
    public static UnityEvent onPlayerDeath = new UnityEvent();
    public static UnityEvent onEnemyDeath = new UnityEvent();

    public static UnityEvent<HealthComponent> OnLifeChanged = new UnityEvent<HealthComponent>();

    public static UnityEvent<bool> OnTurnChanged = new UnityEvent<bool>();

    public static UnityEvent<int, int> OnManaChanged = new UnityEvent<int, int>();

    // ⭐ NUEVO EVENTO PARA INVENTARIO
    public static UnityEvent<CardData> OnCardUnlocked = new UnityEvent<CardData>();
}


