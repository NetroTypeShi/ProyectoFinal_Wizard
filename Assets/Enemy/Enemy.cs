using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Datos del enemigo (ScriptableObject)")]
    public EnemyStats stats;
    public HealthComponent health;

    [Header("Tipo elemental del enemigo")]
    public ElementType tipoEnemigo;

    [Header("UI del enemigo")]
    public GameObject uiPrefab;
    private GameObject uiInstance;
    private UnityEngine.UI.Image tipoIcono;
    private TMPro.TextMeshProUGUI nombreTexto;

    [Header("Iconos por tipo")]
    public Sprite iconFuego;
    public Sprite iconHielo;
    public Sprite iconRayo;
    public Sprite iconAgua;
    public Sprite iconVida;
    public Sprite iconMuerte;

    [Header("IA")]
    public float wanderRadius = 8f;
    public float wanderInterval = 3f;

    private NavMeshAgent agent;
    private float wanderTimer;
    private Vector3 originPos;

    [Header("Sprites")]
    public SpriteRenderer spriteRenderer;

    public Sprite[] upSprites;
    public Sprite[] downSprites;
    public Sprite[] rightSprites;

    public int lastDirection = 0;

    private float animationTimer = 0f;
    public float animationInterval = 0.2f;
    private int animationFrame = 0;

    private bool inBattle = false;

    public int ExpReward => stats.expReward;

    private void Awake()
    {
        if (stats != null && health != null)
        {
            health.maxHealth = stats.maxHealth;
            health.currentHealth = stats.maxHealth;

            tipoEnemigo = stats.tipoEnemigo;
        }
        else
        {
            Debug.LogError("EnemyStats o HealthComponent no asignados en " + name);
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originPos = transform.position;
        wanderTimer = wanderInterval;

        if (uiPrefab != null)
        {
            uiInstance = Instantiate(uiPrefab, transform);
            uiInstance.transform.localPosition = new Vector3(0, 2f, 0);

            tipoIcono = uiInstance.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();
            nombreTexto = uiInstance.transform.Find("NameText").GetComponent<TMPro.TextMeshProUGUI>();

            nombreTexto.text = stats.enemyName;

            switch (tipoEnemigo)
            {
                case ElementType.Fuego: tipoIcono.sprite = iconFuego; break;
                case ElementType.Hielo: tipoIcono.sprite = iconHielo; break;
                case ElementType.Rayo: tipoIcono.sprite = iconRayo; break;
                case ElementType.Agua: tipoIcono.sprite = iconAgua; break;
                case ElementType.Vida: tipoIcono.sprite = iconVida; break;
                case ElementType.Muerte: tipoIcono.sprite = iconMuerte; break;
            }
        }
    }

    void Update()
    {
        if (inBattle)
            return;

        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(originPos, wanderRadius, NavMesh.AllAreas);

            if (agent != null && agent.enabled && agent.isOnNavMesh)
                agent.SetDestination(newPos);

            wanderTimer = 0f;
        }

        Vector3 vel = agent != null ? agent.velocity : Vector3.zero;

        if (vel.magnitude < 0.1f)
        {
            animationFrame = 0;
            animationTimer = 0f;
        }
        else
        {
            float x = vel.x;
            float z = vel.z;

            if (Mathf.Abs(x) > Mathf.Abs(z))
                lastDirection = x > 0 ? 2 : 3;
            else
                lastDirection = z > 0 ? 1 : 0;

            animationTimer += Time.deltaTime;
            if (animationTimer >= animationInterval)
            {
                animationFrame = (animationFrame + 1) % 4;
                animationTimer = 0f;
            }
        }

        switch (lastDirection)
        {
            case 0:
                spriteRenderer.sprite = downSprites[animationFrame];
                spriteRenderer.flipX = false;
                break;
            case 1:
                spriteRenderer.sprite = upSprites[animationFrame];
                spriteRenderer.flipX = false;
                break;
            case 2:
                spriteRenderer.sprite = rightSprites[animationFrame];
                spriteRenderer.flipX = false;
                break;
            case 3:
                spriteRenderer.sprite = rightSprites[animationFrame];
                spriteRenderer.flipX = true;
                break;
        }
    }

    public void LookAtPlayer(Transform player)
    {
        Vector3 dir = player.position - transform.position;

        float x = dir.x;
        float z = dir.z;

        if (Mathf.Abs(x) > Mathf.Abs(z))
            lastDirection = x > 0 ? 2 : 3;
        else
            lastDirection = z > 0 ? 1 : 0;

        animationFrame = 0;

        switch (lastDirection)
        {
            case 0:
                spriteRenderer.sprite = downSprites[0];
                spriteRenderer.flipX = false;
                break;
            case 1:
                spriteRenderer.sprite = upSprites[0];
                spriteRenderer.flipX = false;
                break;
            case 2:
                spriteRenderer.sprite = rightSprites[0];
                spriteRenderer.flipX = false;
                break;
            case 3:
                spriteRenderer.sprite = rightSprites[0];
                spriteRenderer.flipX = true;
                break;
        }
    }

    public void EnterBattle(Transform player)
    {
        inBattle = true;

        if (agent != null)
            agent.enabled = false;

        LookAtPlayer(player);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}




