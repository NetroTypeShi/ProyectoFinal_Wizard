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
                EjecutarCuracion(combate, esJugador);
                break;
        }
    }

    void EjecutarDaño(CombatController combate, bool esJugador)
    {
        if (esJugador)
        {
            // ⭐ Daño del jugador al enemigo
            float mult = TypeChart.GetMultiplier(data.tipo, combate.enemigo.tipoEnemigo);
            int dañoFinal = Mathf.RoundToInt(data.damage * mult);

            combate.enemigoHealth.TakeDamage(dañoFinal);
            GameEvents.OnLifeChanged.Invoke(combate.enemigoHealth);

            if (combate.enemigoHealth.currentHealth > 0)
                combate.MostrarDaño(dañoFinal, combate.enemyDamagePoint, false);

            // ⭐ Aplicar quemadura SOLO al enemigo
            if (data.aplicaQuemadura)
            {
                StatusEffects status = combate.enemigoHealth.GetComponent<StatusEffects>();
                if (status != null)
                    status.ApplyBurn(data.quemaduraDaño, data.quemaduraTurnos);
            }
        }
        else
        {
            // ⭐ Daño del enemigo al jugador
            float mult = TypeChart.GetMultiplier(data.tipo, combate.tipoJugador);
            int dmg = Mathf.RoundToInt(data.damage * mult);

            if (combate.defensaActiva)
                dmg = Mathf.RoundToInt(dmg * (1f - combate.defensaPorcentaje));

            combate.jugadorHealth.TakeDamage(dmg);
            GameEvents.OnLifeChanged.Invoke(combate.jugadorHealth);

            if (combate.jugadorHealth.currentHealth > 0)
                combate.MostrarDaño(dmg, combate.playerDamagePoint, false);

            // ⭐ Aplicar quemadura SOLO al jugador
            if (data.aplicaQuemadura)
            {
                StatusEffects status = combate.jugadorHealth.GetComponent<StatusEffects>();
                if (status != null)
                    status.ApplyBurn(data.quemaduraDaño, data.quemaduraTurnos);
            }
        }
    }

    void EjecutarDefensa(CombatController combate, bool esJugador)
    {
        combate.defensaActiva = true;
        combate.defensaPorcentaje = data.defensePercent;
        combate.defensaTurnosRestantes = data.shieldTurns;

        Transform objetivo = esJugador ?
            combate.jugadorHealth.transform :
            combate.enemigoHealth.transform;

        combate.MostrarEscudo(objetivo);
    }

    void EjecutarCuracion(CombatController combate, bool esJugador)
    {
        Transform objetivo;
        HealthComponent health;

        if (esJugador)
        {
            objetivo = combate.jugadorHealth.transform;
            health = combate.jugadorHealth;

            health.Heal(data.healAmount);
            GameEvents.OnLifeChanged.Invoke(combate.jugadorHealth);
            combate.MostrarDaño(data.healAmount, combate.playerDamagePoint, true);
        }
        else
        {
            objetivo = combate.enemigoHealth.transform;
            health = combate.enemigoHealth;

            health.Heal(data.healAmount);
            GameEvents.OnLifeChanged.Invoke(combate.enemigoHealth);
            combate.MostrarDaño(data.healAmount, combate.enemyDamagePoint, true);
        }

        // ⭐ Partículas de curación
        if (combate.healParticlesPrefab != null)
        {
            GameObject fx = GameObject.Instantiate(
                combate.healParticlesPrefab,
                objetivo.position,
                Quaternion.identity
            );

            GameObject.Destroy(fx, 3f);
        }
    }
}












