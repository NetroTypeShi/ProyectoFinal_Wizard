using UnityEngine;
using TMPro;
using System.Collections;

public class CinematicController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI narrativeText;

    [Header("Texto de la historia")]
    [TextArea(3, 10)]
    public string[] frases;
    public float velocidadEscritura = 0.04f;
    public float tiempoEntreFrases = 2f;
    public bool fastMode;
    [Header("Escena siguiente")]
    public string nextSceneName = "SampleScene";

    private void Start()
    {
        StartCoroutine(ReproducirCinematica());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fastMode = true;
            print("a");
        }
    }

    private IEnumerator ReproducirCinematica()
    {
        foreach (string frase in frases)
        {
            yield return StartCoroutine(EscribirTexto(frase));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator EscribirTexto(string frase)
    {
        narrativeText.text = "";
        fastMode = false;

        for (int i = 0; i < frase.Length; i++)
        {
            narrativeText.text += frase[i];
            float waitTime = fastMode ? 0f : velocidadEscritura;
            yield return new WaitForSeconds(waitTime);
        }
    }



}
// Scale with Screen Size