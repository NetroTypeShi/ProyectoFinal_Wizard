using UnityEngine;

public class ShieldOrbit : MonoBehaviour
{
    public Transform target;
    public float radius = 1.5f;
    public float height = 0.5f;
    public float speed = 90f;

    private float angle;

    void Update()
    {
        if (target == null) return;

        angle += speed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;

        // Órbita horizontal en XZ
        float x = Mathf.Cos(rad) * radius;
        float z = Mathf.Sin(rad) * radius;

        transform.position = target.position + new Vector3(x, height, z);

        // Mirar siempre al centro
        transform.LookAt(target);
    }
}





