using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    public Checkpoint currentCheckpoint;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void ActivateCheckpoint(Checkpoint cp)
    {
        currentCheckpoint = cp;
        Debug.Log("Checkpoint activado: " + cp.name);
    }

    public Vector3 GetRespawnPosition()
    {
        if (currentCheckpoint != null)
            return currentCheckpoint.spawnPoint.position;

        return Vector3.zero; // fallback
    }
}

