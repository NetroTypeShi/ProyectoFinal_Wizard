using UnityEngine;

public class CartaAtaque : MonoBehaviour
{
    public CardData data;

    public void EjecutarCarta(CombatController combate, bool esJugador)
    {
        int roll = Random.Range(0, 100);

        // Probabilidad de fallar la carta
        if (roll < data.failChance)
        {
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

    // ---------------------------
    //        DAÑO
    // ---------------------------

    void EjecutarDaño(CombatController combate, bool esJugador)
    {
        if (esJugador)
        {
            float mult = TypeChart.GetMultiplier(data.tipo, combate.enemigo.tipoEnemigo);
            int dmg = Mathf.RoundToInt(data.damage * mult);

            combate.enemigoHealth.TakeDamage(dmg);
            GameEvents.OnLifeChanged.Invoke(combate.enemigoHealth);

            if (combate.enemigoHealth.currentHealth > 0)
                combate.MostrarDaño(dmg, combate.enemyDamagePoint, false);

            StatusEffects status = combate.enemigoHealth.GetComponent<StatusEffects>();

            if (data.aplicaQuemadura)
                status.ApplyBurn(data.quemaduraDaño, data.quemaduraTurnos);

            if (data.aplicaParalisis)
                AplicarParalisis(status, combate, false);
        }
        else
        {
            float mult = TypeChart.GetMultiplier(data.tipo, combate.tipoJugador);
            int dmg = Mathf.RoundToInt(data.damage * mult);

            if (combate.defensaActiva)
                dmg = Mathf.RoundToInt(dmg * (1f - combate.defensaPorcentaje));

            combate.jugadorHealth.TakeDamage(dmg);
            GameEvents.OnLifeChanged.Invoke(combate.jugadorHealth);

            if (combate.jugadorHealth.currentHealth > 0)
                combate.MostrarDaño(dmg, combate.playerDamagePoint, false);

            StatusEffects status = combate.jugadorHealth.GetComponent<StatusEffects>();

            if (data.aplicaQuemadura)
                status.ApplyBurn(data.quemaduraDaño, data.quemaduraTurnos);

            if (data.aplicaParalisis)
                AplicarParalisis(status, combate, true);
        }
    }

    // ---------------------------
    //        PARÁLISIS
    // ---------------------------

    void AplicarParalisis(StatusEffects status, CombatController combate, bool esJugadorObjetivo)
    {
        int roll = Random.Range(0, 100);

        // Probabilidad de aplicar parálisis
        if (roll < data.paralisisApplyChance)
        {
            status.ApplyParalysis(data.paralisisTurnos, data.paralisisParticles);

            // Cambiar material de la barra
            if (esJugadorObjetivo)
            {
                if (combate.playerHealthBar != null)
                    combate.playerHealthBar.SetParalizado(true);
            }
            else
            {
                if (combate.enemyHealthBar != null)
                    combate.enemyHealthBar.SetParalizado(true);
            }
        }
        else
        {
            combate.MostrarMensaje("La parálisis falló");
        }
    }

    // ---------------------------
    //        DEFENSA
    // ---------------------------

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

    // ---------------------------
    //        CURACIÓN
    // ---------------------------

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

        if (combate.healParticlesPrefab != null)
        {
            GameObject fx = Instantiate(combate.healParticlesPrefab, objetivo.position, Quaternion.identity);
            Destroy(fx, 3f);
        }
    }
}















