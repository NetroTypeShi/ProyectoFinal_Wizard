using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;
    public Checkpoint currentCheckpoint;
    public bool shouldRespawn = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ActivateCheckpoint(Checkpoint cp)
    {
        currentCheckpoint = cp;
        shouldRespawn = true;
        Debug.Log("Checkpoint activado: " + cp.name);
    }

    public Vector3 GetRespawnPosition()
    {
        if (currentCheckpoint != null)
            return currentCheckpoint.transform.position;

        return Vector3.zero;
    }
}


