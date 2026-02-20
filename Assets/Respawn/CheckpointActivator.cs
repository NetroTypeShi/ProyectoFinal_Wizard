using UnityEngine;
using TMPro;
using System.Collections;

public class CheckpointActivator : MonoBehaviour
{
    public Checkpoint checkpoint;
    public TextMeshProUGUI interactText;
    public ParticleSystem activateParticles;

    private bool playerInRange = false;
    private bool activatedOnce = false;

    private void Start()
    {
        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (activatedOnce)
            return;

        if (!playerInRange)
        {
            if (interactText != null)
                interactText.gameObject.SetActive(false);
            return;
        }

        if (interactText != null)
            interactText.gameObject.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckpointManager.instance.ActivateCheckpoint(checkpoint);
            Debug.Log("Checkpoint activado: " + checkpoint.name);

            activatedOnce = true;

            // Retraso de 0.5s antes de reproducir partículas
            StartCoroutine(PlayParticlesDelayed());

            if (interactText != null)
                interactText.gameObject.SetActive(false);

            playerInRange = false;
        }
    }

    private IEnumerator PlayParticlesDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        if (activateParticles != null)
            activateParticles.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activatedOnce)
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }
}




