using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CombatController : MonoBehaviour
{
    [Header("Stats")]
    public HealthComponent jugadorHealth;
    public HealthComponent enemigoHealth;
    public Enemy enemigo;
    public int JugadorMana = 5;
    public int EnemyMana = 5;
    private int maxMana;
    [Header("Tipo elemental del jugador")]
    public ElementType tipoJugador = ElementType.Fuego;
    public DeathScreenUI deathScreen;

    [Header("Defensa")]
    public bool defensaActiva = false;
    public float defensaPorcentaje = 0f;
    public int defensaTurnosRestantes = 0;

    [Header("Mazo del jugador")]
    public DeckManager deck;

    [Header("Mazo del enemigo")]
    public EnemyDeckManager enemyDeck;

    [Header("Visual jugador")]
    public Transform cartasSpawnPoint;
    public GameObject cartaVisualPrefab;
    public float separacion = 2f;
    public float levantamientoY = 0.5f;

    [Header("Canvas Group Cartas")]
    public CanvasGroup cartasGroup;
    public float fadeSpeed = 3f;

    private Coroutine fadeRoutine;

    private List<GameObject> cartasInstanciadas = new List<GameObject>();
    private int currentIndex = 0;
    private bool esperandoSeleccion = false;

    private void Start()
    {
        maxMana = JugadorMana;
        StartCoroutine(EsperarPlayer());

        GameEvents.OnManaChanged.Invoke(JugadorMana, maxMana);
        GameEvents.OnTurnChanged.Invoke(true);

        SetCartasAlpha(1f);
    }

    private IEnumerator EsperarPlayer()
    {
        GameObject playerObj = null;

        while (playerObj == null)
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        jugadorHealth = playerObj.GetComponent<HealthComponent>();
        jugadorHealth.OnDeath += JugadorMuerto;

        jugadorHealth.OnHealthChanged += (vidaActual, vidaMax) =>
        {
            GameEvents.OnLifeChanged.Invoke(jugadorHealth);
        };

        GameEvents.OnLifeChanged.Invoke(jugadorHealth);

        MostrarMano();
        esperandoSeleccion = true;
    }

    private void Update()
    {
        if (!esperandoSeleccion) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = (currentIndex + 1) % cartasInstanciadas.Count;
            MostrarCartaActual();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = cartasInstanciadas.Count - 1;
            MostrarCartaActual();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SeleccionarCarta(currentIndex);
        }
    }

    public void IniciarCombate(GameObject enemigoGO)
    {
        enemigo = enemigoGO.GetComponent<Enemy>();
        enemigoHealth = enemigoGO.GetComponent<HealthComponent>();
        enemigoHealth.OnDeath += EnemigoMuerto;

        enemigoHealth.OnHealthChanged += (vidaActual, vidaMax) =>
        {
            GameEvents.OnLifeChanged.Invoke(enemigoHealth);
        };

        GameEvents.OnLifeChanged.Invoke(enemigoHealth);

        MostrarMano();
        esperandoSeleccion = true;

        GameEvents.OnManaChanged.Invoke(JugadorMana, maxMana);
        GameEvents.OnTurnChanged.Invoke(true);

        SetCartasAlpha(1f);
    }

    private void MostrarMano()
    {
        foreach (var c in cartasInstanciadas)
            Destroy(c);

        cartasInstanciadas.Clear();

        var mano = deck.ObtenerMano();

        for (int i = 0; i < mano.Count; i++)
        {
            CardData data = mano[i];

            GameObject cartaGO = Instantiate(cartaVisualPrefab, cartasSpawnPoint);
            cartaGO.transform.localPosition = new Vector3(i * separacion, 0, 0);

            CartaVisual visual = cartaGO.GetComponent<CartaVisual>();
            visual.Configurar(data);

            cartasInstanciadas.Add(cartaGO);
        }

        MostrarCartaActual();
    }

    private void MostrarCartaActual()
    {
        for (int i = 0; i < cartasInstanciadas.Count; i++)
        {
            var cartaGO = cartasInstanciadas[i];

            if (i == currentIndex)
            {
                cartaGO.transform.localPosition =
                    new Vector3(i * separacion, levantamientoY, 0);
            }
            else
            {
                cartaGO.transform.localPosition =
                    new Vector3(i * separacion, 0, 0);
            }
        }
    }

    private void SeleccionarCarta(int index)
    {
        if (!esperandoSeleccion) return;

        var manoJugador = deck.ObtenerMano();
        if (manoJugador.Count == 0 || index < 0 || index >= manoJugador.Count)
            return;

        CardData carta = manoJugador[index];

        if (JugadorMana < carta.manaCost)
        {
            Debug.Log("No tienes suficiente mana");
            return;
        }

        JugadorMana -= carta.manaCost;
        GameEvents.OnManaChanged.Invoke(JugadorMana, maxMana);

        CartaVisual visual = cartasInstanciadas[index].GetComponent<CartaVisual>();
        visual.cartaLogic.EjecutarCarta(this, true);

        tipoJugador = carta.tipo;

        deck.UsarCarta(index);

        MostrarMano();

        esperandoSeleccion = false;
        ComprobarEstado();
    }

    private void ComprobarEstado()
    {
        if (enemigoHealth != null && enemigoHealth.currentHealth <= 0)
        {
            GameEvents.onEnemyDeath.Invoke();
            return;
        }

        if (jugadorHealth != null && jugadorHealth.currentHealth <= 0)
        {
            GameEvents.onPlayerDeath.Invoke();
            return;
        }

        TurnoEnemigo();
    }

    private void TurnoEnemigo()
    {
        GameEvents.OnTurnChanged.Invoke(false);

        SetCartasAlpha(0f);

        StartCoroutine(TurnoEnemigoCoroutine());
    }

    private IEnumerator TurnoEnemigoCoroutine()
    {
        yield return new WaitForSeconds(3f);

        var manoEnemigo = enemyDeck.ObtenerMano();

        if (manoEnemigo == null || manoEnemigo.Count == 0)
        {
            TurnoJugador();
            yield break;
        }

        int index = Random.Range(0, manoEnemigo.Count);
        CardData carta = manoEnemigo[index];

        if (EnemyMana < carta.manaCost)
        {
            TurnoJugador();
            yield break;
        }

        EnemyMana -= carta.manaCost;

        CartaAtaque temp = new GameObject("TempCard").AddComponent<CartaAtaque>();
        temp.data = carta;
        temp.EjecutarCarta(this, false);
        Destroy(temp.gameObject);

        enemyDeck.UsarCarta(index);

        if (jugadorHealth.currentHealth <= 0)
            yield break;

        TurnoJugador();
    }

    private void TurnoJugador()
    {
        GameEvents.OnTurnChanged.Invoke(true);

        esperandoSeleccion = true;
        MostrarCartaActual();

        SetCartasAlpha(1f);
    }

    private void EnemigoMuerto()
    {
        Debug.Log("El enemigo ha muerto");
    }

    private void JugadorMuerto()
    {
        Debug.Log("Has muerto");
        deathScreen.ShowDeathScreen();

        StartCoroutine(VolverAlMundo());
    }

    private IEnumerator VolverAlMundo()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("SampleScene"); 
    }

    private void SetCartasAlpha(float target)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeCartas(target));
    }

    private IEnumerator FadeCartas(float target)
    {
        while (!Mathf.Approximately(cartasGroup.alpha, target))
        {
            cartasGroup.alpha = Mathf.Lerp(cartasGroup.alpha, target, Time.deltaTime * fadeSpeed);
            yield return null;
        }

        cartasGroup.alpha = target;
    }
}




