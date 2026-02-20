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

    private bool fastMode = false;
    private bool fraseTerminada = false;

    [Header("Escena siguiente")]
    public string nextSceneName = "SampleScene";

    [Header("Transición épica")]
    public CanvasGroup panelTransicion;     // Panel negro con CanvasGroup
    public float duracionFade = 1.5f;
    public ParticleSystem particulasEpicas; // opcional
    public AudioSource sonidoEpico;         // opcional

    [Header("Música")]
    public AudioSource musica;              // La música de fondo
    public float duracionFadeMusica = 2f;   // Tiempo para bajar el volumen

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            fastMode = true;
    }

    private void Start()
    {
        StartCoroutine(ReproducirCinematica());
    }

    private IEnumerator ReproducirCinematica()
    {
        foreach (string frase in frases)
        {
            yield return StartCoroutine(EscribirTexto(frase));

            float timer = 0f;
            while (timer < tiempoEntreFrases)
            {
                if (Input.GetKey(KeyCode.Space))
                    break;

                timer += Time.deltaTime;
                yield return null;
            }

            fastMode = false;
        }

        // ⭐ Primero bajamos la música
        yield return StartCoroutine(FadeOutMusica());

        // ⭐ Luego hacemos la transición épica
        yield return StartCoroutine(TransicionEpica());

        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator EscribirTexto(string frase)
    {
        narrativeText.text = "";
        fraseTerminada = false;

        for (int i = 0; i < frase.Length; i++)
        {
            narrativeText.text += frase[i];

            float wait = fastMode ? 0f : velocidadEscritura;
            yield return new WaitForSeconds(wait);
        }

        fraseTerminada = true;
    }

    private IEnumerator TransicionEpica()
    {
        if (particulasEpicas != null)
            particulasEpicas.Play();

        if (sonidoEpico != null)
            sonidoEpico.Play();

        float t = 0f;

        while (t < duracionFade)
        {
            t += Time.deltaTime;
            float p = t / duracionFade;

            panelTransicion.alpha = Mathf.Lerp(0f, 1f, p);

            yield return null;
        }
    }

    private IEnumerator FadeOutMusica()
    {
        if (musica == null)
            yield break;

        float volumenInicial = musica.volume;
        float t = 0f;

        while (t < duracionFadeMusica)
        {
            t += Time.deltaTime;
            float p = t / duracionFadeMusica;

            musica.volume = Mathf.Lerp(volumenInicial, 0f, p);

            yield return null;
        }

        musica.Stop();
    }
}



// Scale with Screen Size