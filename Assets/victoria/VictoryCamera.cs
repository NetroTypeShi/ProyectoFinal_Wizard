using UnityEngine;
using System.Collections;

public class VictoryCamera : MonoBehaviour
{
    public static VictoryCamera Instance;

    private Camera cam;
    private bool active = false;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    public void MoveCameraToVictory(Transform player)
    {
        if (active) return;
        active = true;

        StartCoroutine(MoveRoutine(player));
    }

    private IEnumerator MoveRoutine(Transform player)
    {
        Vector3 startPos = cam.transform.position;
        Quaternion startRot = cam.transform.rotation;

        // ⭐ Posición delante del jugador, un poco a la derecha
        Vector3 targetPos = player.position
                            + player.forward * 2.5f
                            + player.right * 1.2f
                            + Vector3.up * 1.5f;

        Quaternion targetRot = Quaternion.LookRotation(player.position - targetPos);

        float t = 0;
        float duration = 1f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            cam.transform.position = Vector3.Lerp(startPos, targetPos, p);
            cam.transform.rotation = Quaternion.Slerp(startRot, targetRot, p);

            yield return null;
        }
    }
}

