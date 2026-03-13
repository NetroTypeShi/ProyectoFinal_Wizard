using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogoNPC : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI textoNombre;

    [Header("Respuesta Si/No")]
    public GameObject panelRespuesta;
    public Button botonSi;
    public Button botonNo;

    [Header("Escena")]
    public string escenaBatalla = "BattleScene";

    [Header("Contenido")]
    public string nombreNPC = "Aldeano";
    [TextArea(3, 10)]
    public string[] frases;

    [Header("Configuración")]
    public float velocidadEscritura = 0.04f;
    public float distanciaInteraccion = 3f;
    public KeyCode teclaInteraccion = KeyCode.E;

    [Header("Indicador")]
    public GameObject indicadorE;

    private bool jugadorCerca = false;
    private bool dialogoActivo = false;
    private bool esperandoRespuesta = false;
    private Transform jugador;
    private PlayerMovement playerMovement;
    private int opcionSeleccionada = 0;

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = jugador.GetComponent<PlayerMovement>();
        panelDialogo.SetActive(false);
        panelRespuesta.SetActive(false);
        if (indicadorE != null)
            indicadorE.SetActive(false);

        botonSi.onClick.AddListener(RespuestaSi);
        botonNo.onClick.AddListener(RespuestaNo);
    }

    

    private void Update()
    {
        float distancia = Vector3.Distance(transform.position, jugador.position);
        jugadorCerca = distancia <= distanciaInteraccion;

        if (indicadorE != null)
            indicadorE.SetActive(jugadorCerca && !dialogoActivo);

        if (jugadorCerca && !dialogoActivo && Input.GetKeyDown(teclaInteraccion))
            StartCoroutine(ReproducirDialogo());

        // Manejo de selección con teclado cuando el panel de respuesta está activo
        if (panelRespuesta.activeSelf && esperandoRespuesta)
        {
            // Cambiar selección con flechas
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W))
            {
                opcionSeleccionada = 0;
                botonSi.Select();
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S))
            {
                opcionSeleccionada = 1;
                botonNo.Select();
            }

            // Confirmar selección con Enter
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (opcionSeleccionada == 0)
                    RespuestaSi();
                else
                    RespuestaNo();
            }
        }
    }

    // Al mostrar el panel de respuesta, selecciona por defecto el botón "Sí"
    private IEnumerator ReproducirDialogo()
    {
        dialogoActivo = true;
        panelDialogo.SetActive(true);
        if (playerMovement != null) playerMovement.bloqueado = true;
        if (textoNombre != null) textoNombre.text = nombreNPC;

        for (int i = 0; i < frases.Length; i++)
        {
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.Space));
            yield return StartCoroutine(EscribirTexto(frases[i]));

            if (i == frases.Length - 1)
            {
                esperandoRespuesta = true;
                panelRespuesta.SetActive(true);
                opcionSeleccionada = 0;
                botonSi.Select(); // Selecciona "Sí" por defecto
                yield return new WaitUntil(() => !esperandoRespuesta);
            }
            else
            {
                yield return new WaitUntil(() => !Input.GetKey(KeyCode.Space));
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            }
        }

        panelDialogo.SetActive(false);
        if (playerMovement != null) playerMovement.bloqueado = false;
        dialogoActivo = false;
    }


    private IEnumerator EscribirTexto(string frase)
    {
        textoDialogo.text = "";

        for (int i = 0; i < frase.Length; i++)
        {
            // Si pulsan Space durante la escritura, mostrar frase completa
            if (Input.GetKey(KeyCode.Space))
            {
                textoDialogo.text = frase;
                break;
            }
            textoDialogo.text += frase[i];
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }
    private void RespuestaSi()
    {
        panelRespuesta.SetActive(false);
        esperandoRespuesta = false;
        SceneManager.LoadScene(escenaBatalla);
    }

    private void RespuestaNo()
    {
        panelRespuesta.SetActive(false);
        esperandoRespuesta = false;
        panelDialogo.SetActive(false);
        if (playerMovement != null) playerMovement.bloqueado = false;
        dialogoActivo = false;
    }
}
