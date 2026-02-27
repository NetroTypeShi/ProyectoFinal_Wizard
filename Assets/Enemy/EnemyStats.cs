using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Combat/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public string enemyName;
    public int maxHealth;
    public int maxMana;
    public Sprite enemySprite;

    [Header("Tipo elemental del enemigo")]
    public ElementType tipoEnemigo;

    [Header("Mazo del enemigo")]
    public List<CardData> mazoBase;

    [Header("Recompensas")]
    public int expReward = 10;   // ⭐ EXP que da este enemigo
}
