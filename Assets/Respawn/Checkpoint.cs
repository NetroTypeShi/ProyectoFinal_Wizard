using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform spawnPoint; // Lugar exacto donde reaparece el jugador

    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawnPoint.position, 0.3f);
        }
    }
}

