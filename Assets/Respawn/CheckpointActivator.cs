using UnityEngine;
using TMPro;

public class CheckpointActivator : MonoBehaviour
{
    public Checkpoint checkpoint;
    public TextMeshProUGUI interactText;
    public ParticleSystem activateParticles;

    private bool playerInRange = false;
    private bool activatedOnce = false;   // ← controla si ya se activó por primera vez

    private void Start()
    {
        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Si ya se activó una vez, nunca mostramos el texto
        if (activatedOnce)
            return;

        if (!playerInRange)
        {
            if (interactText != null)
                interactText.gameObject.SetActive(false);
            return;
        }

        // Mostrar texto mientras está cerca
        if (interactText != null)
            interactText.gameObject.SetActive(true);

        // Activar checkpoint
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckpointManager.instance.ActivateCheckpoint(checkpoint);
            Debug.Log("Checkpoint activado: " + checkpoint.name);

            // SOLO LA PRIMERA VEZ
            activatedOnce = true;

            // Partículas
            if (activateParticles != null)
                activateParticles.Play();

            // Ocultar texto para siempre
            if (interactText != null)
                interactText.gameObject.SetActive(false);

            // Evitar que vuelva a salir
            playerInRange = false;
        }
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



