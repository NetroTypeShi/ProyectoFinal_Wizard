using UnityEngine;

public class PickupCard : MonoBehaviour
{
    [Header("Carta que desbloquea este objeto")]
    public CardData cartaAEntregar;

    [Header("Rotación del objeto")]
    public float rotationSpeed = 60f;

    [Header("Referencia al texto de UI (desactivado por defecto)")]
    public GameObject popupUI; // Tu TextMeshPro en la UI

    private void Update()
    {
        // Rotación estilo moneda
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Desbloquear carta
        DeckManager.instance.DesbloquearCarta(cartaAEntregar);

        // Activar popup de UI
        if (popupUI != null)
        {
            popupUI.SetActive(true);

            // Lanzar animación
            PopupUIAnim anim = popupUI.GetComponent<PopupUIAnim>();
            if (anim != null)
                anim.Play();
        }

        // Destruir el objeto recogible
        Destroy(gameObject);
    }
}




