using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public static PlayerExperience Instance;

    [Header("Nivel")]
    public int level = 1;

    [Header("Experiencia")]
    public int currentExp = 0;
    public int expToNextLevel = 20;

    [Header("Escalado de experiencia")]
    [Tooltip("Multiplicador para aumentar la experiencia necesaria por nivel")]
    public float expGrowthMultiplier = 1.25f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddExperience(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expGrowthMultiplier);
        Debug.Log("¡Subiste a nivel " + level + "!");
    }
}

