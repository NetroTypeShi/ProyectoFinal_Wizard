using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    [Header("🔥 Quemadura")]
    public int burnDamage = 0;
    public int burnTurns = 0;

    [Header("⚡ Parálisis")]
    public int paralizadoTurnos = 0;

    [Tooltip("Probabilidad por turno de que la parálisis impida actuar")]
    [Range(0, 100)]
    public int paralisisTurnFailChance = 25;

    public GameObject paralisisParticles;

    // ---------------------------
    //        QUEMADURA
    // ---------------------------

    public void ApplyBurn(int dmg, int turns)
    {
        burnDamage = dmg;
        burnTurns = turns;
    }

    public bool HasBurn()
    {
        return burnTurns > 0 && burnDamage > 0;
    }

    public int TickBurn()
    {
        if (!HasBurn())
            return 0;

        burnTurns--;
        return burnDamage;
    }

    // ---------------------------
    //        PARÁLISIS
    // ---------------------------

    public void ApplyParalysis(int turns, GameObject particles)
    {
        paralizadoTurnos = turns;
        paralisisParticles = particles;

        if (particles != null)
        {
            GameObject fx = Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(fx, 2f);
        }
    }

    public bool EstaParalizado()
    {
        return paralizadoTurnos > 0;
    }

    public void TickTurn()
    {
        if (paralizadoTurnos > 0)
            paralizadoTurnos--;
    }
}
