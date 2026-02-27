using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyBattleStarter : MonoBehaviour
{
    [Header("Detección y combate")]
    public float detectionDistance = 6f;
    public float engageDistance = 1.5f;
    public float maxChaseDistance = 10f;
    public string battleSceneName = "BattleScene";

    [Header("Velocidades")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Exclamación")]
    public GameObject alertIconPrefab;
    public float alertDuration = 0.6f;

    private EnemyVision vision;
    private NavMeshAgent agent;
    private Transform player;
    private PlayerMovement playerMovement;

    private bool chasing = false;
    private bool alertShown = false;

    private Vector3 originalPosition;

    private void Start()
    {
        vision = GetComponent<EnemyVision>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();

        agent.speed = patrolSpeed;
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (!chasing)
        {
            if (vision.CanSeePlayer(player))
            {
                chasing = true;
                agent.speed = chaseSpeed;

                if (!alertShown)
                {
                    alertShown = true;
                    StartCoroutine(ShowAlertIcon());
                }
            }
            else
                return;
        }

        agent.SetDestination(player.position);

        float chaseDistance = Vector3.Distance(transform.position, player.position);
        if (chaseDistance > maxChaseDistance)
        {
            StopChasing();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= engageDistance && chasing)
        {
            StartBattle();
        }
    }

    private void StopChasing()
    {
        chasing = false;
        alertShown = false;
        agent.speed = patrolSpeed;

        agent.SetDestination(originalPosition);
    }

    private void StartBattle()
    {
        agent.isStopped = true;

        playerMovement.enabled = false;

        // Guardamos este enemigo para la escena de combate
        EnemyStateManager.enemigoSeleccionado = this.gameObject;

        SceneFader fader = FindFirstObjectByType<SceneFader>();
        fader.FadeToScene(battleSceneName);
    }

    private IEnumerator ShowAlertIcon()
    {
        GameObject icon = Instantiate(alertIconPrefab, transform);
        icon.transform.localPosition = new Vector3(0, 2f, 0);

        yield return new WaitForSeconds(alertDuration);

        Destroy(icon);
    }
}
