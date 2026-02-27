using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private void Awake()
    {
        // Evitar duplicados si vuelves a cargar la escena
        var objs = FindObjectsOfType<PersistentUI>();
        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}

