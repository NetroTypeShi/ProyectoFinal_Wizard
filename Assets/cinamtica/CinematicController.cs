using UnityEngine;
using TMPro;
using System.Collections;

public class CinematicController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI narrativeText;

    [Header("Texto de la historia")]
    [TextArea(3, 10)]
    public string[] frases; // ← Aquí escribes cada frase de la historia

    public float velocidadEscritura = 0.04f;
    public float tiempoEntreFrases = 2f;

    [Header("Escena siguiente")]
    public string nextSceneName = "SampleScene";

    private void Start()
    {
        StartCoroutine(ReproducirCinematica());
    }

    private IEnumerator ReproducirCinematica()
    {
        foreach (string frase in frases)
        {
            yield return StartCoroutine(EscribirTexto(frase));
            yield return new WaitForSeconds(tiempoEntreFrases);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator EscribirTexto(string frase)
    {
        narrativeText.text = "";

        foreach (char c in frase)
        {
            narrativeText.text += c;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }
}
