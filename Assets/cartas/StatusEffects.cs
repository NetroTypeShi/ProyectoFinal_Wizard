using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    public int burnDamage = 0;
    public int burnTurns = 0;

    public void ApplyBurn(int damage, int turns)
    {
        burnDamage = damage;
        burnTurns = turns;
    }

    public int TickBurn()
    {
        if (burnTurns > 0)
        {
            burnTurns--;
            return burnDamage;
        }

        return 0;
    }

    public bool HasBurn()
    {
        return burnTurns > 0;
    }
}

