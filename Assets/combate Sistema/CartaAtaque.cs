using UnityEngine;

public class CartaAtaque : MonoBehaviour
{
    public CardData data;

    public void EjecutarCarta(CombatController combate, bool esJugador)
    {
        int roll = Random.Range(0, 100);

        if (roll < data.failChance)
        {
            if (combate != null)
                combate.MostrarMensaje("¡Fallo!");
            return;
        }

        switch (data.type)
        {
            case CardType.Damage:
                EjecutarDaño(combate, esJugador);
                break;

            case CardType.Defense:
                EjecutarDefensa(combate, esJugador);
                break;

            case CardType.Heal:
                EjecutarCuracion(combate);
                break;
        }
    }

    void EjecutarDaño(CombatController combate, bool esJugador)
    {
        if (esJugador)
        {
            float mult = TypeChart.GetMultiplier(data.tipo, combate.enemigo.tipoEnemigo);
            int dañoFinal = Mathf.RoundToInt(data.damage * mult);

            combate.enemigoHealth.TakeDamage(dañoFinal);

            // ⭐ Actualizar vida del enemigo
            GameEvents.OnLifeChanged.Invoke(combate.enemigoHealth);

            if (combate.enemigoHealth.currentHealth > 0)
                combate.MostrarDaño(dañoFinal, combate.enemyDamagePoint, false);
        }
        else
        {
            float mult = TypeChart.GetMultiplier(data.tipo, combate.tipoJugador);
            int dmg = Mathf.RoundToInt(data.damage * mult);

            // ⭐ Aplicar defensa SIN restar turnos aquí
            if (combate.defensaActiva)
            {
                dmg = Mathf.RoundToInt(dmg * (1f - combate.defensaPorcentaje));
            }

            combate.jugadorHealth.TakeDamage(dmg);

            // ⭐ Actualizar vida del jugador
            GameEvents.OnLifeChanged.Invoke(combate.jugadorHealth);

            if (combate.jugadorHealth.currentHealth > 0)
                combate.MostrarDaño(dmg, combate.playerDamagePoint, false);
        }
    }

    void EjecutarDefensa(CombatController combate, bool esJugador)
    {
        combate.defensaActiva = true;
        combate.defensaPorcentaje = data.defensePercent;

        // ⭐ Duración del escudo según la carta
        combate.defensaTurnosRestantes = data.shieldTurns;

        // Target 3D real
        Transform objetivo = esJugador ?
            combate.jugadorHealth.transform :
            combate.enemigoHealth.transform;

        combate.MostrarEscudo(objetivo);
    }

    void EjecutarCuracion(CombatController combate)
    {
        combate.jugadorHealth.Heal(data.healAmount);

        // ⭐ Actualizar vida del jugador
        GameEvents.OnLifeChanged.Invoke(combate.jugadorHealth);

        combate.MostrarDaño(data.healAmount, combate.playerDamagePoint, true);
    }
}











