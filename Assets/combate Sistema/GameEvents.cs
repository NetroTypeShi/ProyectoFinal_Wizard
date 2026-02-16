using UnityEngine;
using UnityEngine.Events;

public static class GameEvents
{
    // Eventos existentes
    public static UnityEvent onRestart = new UnityEvent();
    public static UnityEvent onPlayerDeath = new UnityEvent();
    public static UnityEvent onEnemyDeath = new UnityEvent();
    // Vida (se envía el HealthComponent que cambió)
    public static UnityEvent<HealthComponent> OnLifeChanged = new UnityEvent<HealthComponent>();

    // Turnos (true = jugador, false = enemigo)
    public static UnityEvent<bool> OnTurnChanged = new UnityEvent<bool>();

    // Maná (manaActual, manaMax)
    public static UnityEvent<int, int> OnManaChanged = new UnityEvent<int, int>();
}

