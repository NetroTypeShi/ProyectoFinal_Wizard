using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Combat/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Identidad")]
    public string enemyName;

    [Header("Vida y Mana")]
    public int maxHealth;
    public int maxMana;

    [Header("Sprite del enemigo (mundo)")]
    public Sprite enemySprite;

    [Header("Tipo elemental del enemigo")]
    public ElementType tipoEnemigo;

    [Header("Mazo del enemigo")]
    public List<CardData> mazoBase;

    [Header("Recompensas")]
    public int expReward = 10;

    [Header("Prefab para la batalla (OBLIGATORIO)")]
    public GameObject battlePrefab;
}

