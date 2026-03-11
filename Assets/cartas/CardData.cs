using UnityEngine;

public enum CardType
{
    Damage,
    Defense,
    Heal
}

[CreateAssetMenu(fileName = "CardData", menuName = "Combat/Card")]
public class CardData : ScriptableObject
{
    [Header("Información general")]
    public string cardName;
    public Sprite artwork;
    public int manaCost;

    [Header("Daño")]
    public int damage;

    [Header("Curación")]
    public int healAmount;

    [Header("Defensa")]
    [Tooltip("Porcentaje de daño reducido (0.4 = 40%)")]
    public float defensePercent = 0.4f;

    [Tooltip("Cuántos turnos dura el escudo")]
    public int shieldTurns = 3;

    [Header("Tipo de carta")]
    public CardType type;

    [Header("Tipo elemental")]
    public ElementType tipo;

    [Header("Probabilidad de fallo de la carta")]
    [Range(0, 100)]
    public int failChance = 0;

    [Header("Efectos de estado: Quemadura")]
    public bool aplicaQuemadura = false;
    public int quemaduraDaño = 5;
    public int quemaduraTurnos = 3;

    [Header("Efectos de estado: Parálisis")]
    public bool aplicaParalisis = false;

    [Tooltip("Probabilidad de aplicar parálisis si la carta acierta")]
    [Range(0, 100)]
    public int paralisisApplyChance = 80;

    [Tooltip("Cuántos turnos dura la parálisis")]
    public int paralisisTurnos = 4;

    [Header("Partículas de parálisis")]
    public GameObject paralisisParticles;
}





