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

    //[Me lo pongo de anotación]Cuando usas una carta de daño hay probabilidad de que no haya daño, se cambia a un porcentaje y cada que se use hace un numero al azar entre [ese porcentaje respecto del daño base] arriba a abajo
    [Tooltip("Porcentaje de variabilidad del daño (100% = 0 a daño base, 50% = 0 a 50% del daño base)")]
    [Range(0, 200)]
    public float damageVariabilityPercent = 200f;

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

    [Header("Porcentaje de fallo de la carta")]
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
