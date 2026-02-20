using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 100;
    public int currentHealth = 0;

    public System.Action OnDeath;
    public System.Action<int> OnDamage;
    public System.Action<int, int> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;

        // Evento interno
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Evento global
        GameEvents.OnLifeChanged.Invoke(this);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        // Evento interno
        OnDamage?.Invoke(currentHealth);

        // Evento interno
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Evento global
        GameEvents.OnLifeChanged.Invoke(this);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        // Evento interno
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Evento global
        GameEvents.OnLifeChanged.Invoke(this);
    }

    private void Die()
    {
        OnDeath?.Invoke();

        if (CompareTag("Player"))
        {
            if (CheckpointManager.instance != null)
                CheckpointManager.instance.shouldRespawn = true;

            GameEvents.onPlayerDeath.Invoke();
        }
        else if (CompareTag("Enemy"))
        {
            GameEvents.onEnemyDeath.Invoke();
        }
    }


}
