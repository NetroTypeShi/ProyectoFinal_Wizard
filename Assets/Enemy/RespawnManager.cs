using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void StartRespawn(GameObject enemy)
    {
        StartCoroutine(RespawnCoroutine(enemy));
    }

    private IEnumerator RespawnCoroutine(GameObject enemy)
    {
        float tiempoRestante = EnemyStateManager.respawnTime - Time.time;

        yield return new WaitForSeconds(tiempoRestante);

        EnemyStateManager.enemyDead = false;
        enemy.SetActive(true);
    }
}

