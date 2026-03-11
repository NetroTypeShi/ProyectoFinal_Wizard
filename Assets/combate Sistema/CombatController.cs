using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CombatController : MonoBehaviour
{
    [Header("Datos del enemigo")]
    public EnemyDataCarrier dataCarrier;

    [Header("Spawn Points")]
    public Transform enemySpawnPoint;
    public Transform playerSpawnPoint;

    [Header("Barras de vida")]
    public HealthBarUI playerHealthBar;
    public HealthBarUI enemyHealthBar;

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
    public DeckManager deck => DeckManager.instance;

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

    [Header("UI Mensajes")]
    public TextMeshProUGUI failText;

    [Header("UI Victoria")]
    public TextMeshProUGUI victoryText;
    public GameObject victoryPanel;
    public ExperienceBar victoryExpBar;

    [Header("UI Combate")]
    public GameObject combatUI;

    [Header("Damage Popup")]
    public GameObject damagePopupPrefab;
    public GameObject burnPopupPrefab;
    public Canvas canvas;

    [Header("Damage Popup Targets")]
    public Transform enemyDamagePoint;
    public Transform playerDamagePoint;

    [Header("Puntos de quemadura")]
    public Transform enemyBurnPoint;
    public Transform playerBurnPoint;

    [Header("Escudo")]
    public GameObject shieldOrbitPrefab;
    private GameObject shieldActivo;

    [Header("Partículas")]
    public GameObject healParticlesPrefab;
    public GameObject burnTickParticlesPrefab;

    [Header("Tutorial")]
    public TutorialManager tutorialManager;

    private GameObject enemigoInstanciado;
    private Coroutine fadeRoutine;
    private List<GameObject> cartasInstanciadas = new List<GameObject>();
    private int currentIndex = 0;
    public bool esperandoSeleccion = false;
    private bool paralizadoActivo = false;
    private string cartaPermitida = null;

    private void Start()
    {
        maxMana = JugadorMana;

        deck.ActualizarMazoInicial();
        deck.ReiniciarMazo();

        StartCoroutine(EsperarPlayer());
        InstanciarEnemigo();

        if (failText != null) failText.gameObject.SetActive(false);
        if (victoryText != null) victoryText.gameObject.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    private void InstanciarEnemigo()
    {
        enemigoInstanciado = Instantiate(
            dataCarrier.stats.battlePrefab,
            enemySpawnPoint.position,
            enemySpawnPoint.rotation
        );

        enemigo = enemigoInstanciado.GetComponent<Enemy>();
        enemigoHealth = enemigoInstanciado.GetComponent<HealthComponent>();

        enemigo.stats = dataCarrier.stats;
        enemigoHealth.maxHealth = dataCarrier.stats.maxHealth;
        enemigoHealth.currentHealth = dataCarrier.stats.maxHealth;

        StatusEffects st = enemigoHealth.GetComponent<StatusEffects>();
        if (st != null)
        {
            st.burnTurns = 0;
            st.burnDamage = 0;
            st.paralizadoTurnos = 0;
        }

        enemyHealthBar.SetTarget(enemigoHealth);
        GameEvents.OnLifeChanged.Invoke(enemigoHealth);

        enemigo.tipoEnemigo = dataCarrier.stats.tipoEnemigo;

        enemyDeck.enemyStats = dataCarrier.stats;
        enemyDeck.ReiniciarMazo();

        enemigoHealth.OnDeath += EnemigoMuerto;

        MostrarMano();
        esperandoSeleccion = true;

        GameEvents.OnManaChanged.Invoke(JugadorMana, maxMana);
        GameEvents.OnTurnChanged.Invoke(true);

        cartasGroup.alpha = 1f;
        cartasGroup.interactable = true;
        cartasGroup.blocksRaycasts = true;
    }

    private IEnumerator EsperarPlayer()
    {
        GameObject playerObj = null;

        while (playerObj == null)
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        if (playerSpawnPoint != null)
        {
            playerObj.transform.position = playerSpawnPoint.position;
            playerObj.transform.rotation = playerSpawnPoint.rotation;
        }

        jugadorHealth = playerObj.GetComponent<HealthComponent>();
        jugadorHealth.OnDeath += JugadorMuerto;

        jugadorHealth.currentHealth = jugadorHealth.maxHealth;

        StatusEffects st = jugadorHealth.GetComponent<StatusEffects>();
        if (st != null)
        {
            st.burnTurns = 0;
            st.burnDamage = 0;
            st.paralizadoTurnos = 0;
        }

        playerHealthBar.SetTarget(jugadorHealth);
        GameEvents.OnLifeChanged.Invoke(jugadorHealth);

        jugadorHealth.OnHealthChanged += (vidaActual, vidaMax) =>
        {
            GameEvents.OnLifeChanged.Invoke(jugadorHealth);
        };

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
                cartaGO.transform.localPosition = new Vector3(i * separacion, levantamientoY, 0);
            else
                cartaGO.transform.localPosition = new Vector3(i * separacion, 0, 0);
        }
    }

    private void SeleccionarCarta(int index)
    {
        if (!esperandoSeleccion) return;

        var manoJugador = deck.ObtenerMano();
        if (manoJugador.Count == 0 || index < 0 || index >= manoJugador.Count)
            return;

        if (cartaPermitida != null)
        {
            CardData cartaElegida = manoJugador[index];
            if (cartaElegida.cardName != cartaPermitida)
                return;
        }

        CardData carta = manoJugador[index];

        StatusEffects st = jugadorHealth != null ? jugadorHealth.GetComponent<StatusEffects>() : null;
        if (st != null && st.EstaParalizado())
        {
            int roll = Random.Range(0, 100);
            bool skipeado = roll < st.paralisisTurnFailChance;

            st.TickTurn();
            if (!st.EstaParalizado())
                playerHealthBar.SetParalizado(false);

            if (skipeado)
            {
                if (JugadorMana >= carta.manaCost)
                {
                    JugadorMana -= carta.manaCost;
                    GameEvents.OnManaChanged.Invoke(JugadorMana, maxMana);
                }
                deck.UsarCarta(index);
                MostrarMano();

                if (st.paralisisParticles != null)
                {
                    GameObject fx = Instantiate(st.paralisisParticles, jugadorHealth.transform.position, Quaternion.identity);
                    Destroy(fx, 2f);
                }

                MostrarMensaje("¡Paralizado!");
                StartCoroutine(FinDeAccionJugador());
                return;
            }
        }

        if (JugadorMana < carta.manaCost)
        {
            Debug.Log("No tienes suficiente mana");
            return;
        }

        JugadorMana -= carta.manaCost;
        GameEvents.OnManaChanged.Invoke(JugadorMana, maxMana);

        CartaVisual visual = cartasInstanciadas[index].GetComponent<CartaVisual>();
        visual.cartaLogic.EjecutarCarta(this, true);

        deck.UsarCarta(index);
        MostrarMano();

        if (tutorialManager != null)
            tutorialManager.OnCartaUsada(carta);

        StartCoroutine(FinDeAccionJugador());
    }

    private IEnumerator FinDeAccionJugador()
    {
        esperandoSeleccion = false;

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);

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

        cartasGroup.interactable = false;
        cartasGroup.blocksRaycasts = false;

        StartCoroutine(TurnoEnemigoCoroutine());
    }

    private IEnumerator TurnoEnemigoCoroutine()
    {
        StatusEffects st = enemigoHealth.GetComponent<StatusEffects>();

        if (st != null && st.HasBurn())
        {
            int burn = st.TickBurn();
            if (burn > 0)
            {
                enemigoHealth.TakeDamage(burn);
                MostrarDaño(burn, enemyBurnPoint, false, true);
                SpawnBurnParticles(enemyBurnPoint);
                GameEvents.OnLifeChanged.Invoke(enemigoHealth);
            }
        }

        yield return new WaitForSeconds(1f);

        if (st != null && st.EstaParalizado())
        {
            int roll = Random.Range(0, 100);

            if (roll < st.paralisisTurnFailChance)
            {
                if (st.paralisisParticles != null)
                {
                    GameObject fx = Instantiate(st.paralisisParticles, enemigoHealth.transform.position, Quaternion.identity);
                    Destroy(fx, 2f);
                }

                st.TickTurn();

                if (!st.EstaParalizado())
                    enemyHealthBar.SetParalizado(false);

                yield return new WaitForSeconds(0.6f);
                StartCoroutine(TurnoJugadorCoroutine());
                yield break;
            }
            else
            {
                st.TickTurn();

                if (!st.EstaParalizado())
                    enemyHealthBar.SetParalizado(false);
            }
        }

        var mano = enemyDeck.ObtenerMano();
        if (mano == null || mano.Count == 0)
        {
            StartCoroutine(TurnoJugadorCoroutine());
            yield break;
        }

        int index = Random.Range(0, mano.Count);
        CardData carta = mano[index];

        if (EnemyMana < carta.manaCost)
        {
            StartCoroutine(TurnoJugadorCoroutine());
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

        ReducirTurnosDefensa();

        if (tutorialManager != null)
            tutorialManager.OnEnemigoAtaco();

        // Solo continuar al turno jugador si el tutorial no está mostrando imagen
        if (tutorialManager == null || !tutorialManager.tutorialActivo || tutorialManager.paso != 2)
            StartCoroutine(TurnoJugadorCoroutine());
    }

    private void TurnoJugador()
    {
        StartCoroutine(TurnoJugadorCoroutine());
    }

    private IEnumerator TurnoJugadorCoroutine()
    {
        StatusEffects st = jugadorHealth.GetComponent<StatusEffects>();

        if (st != null && st.HasBurn())
        {
            int burn = st.TickBurn();
            if (burn > 0)
            {
                jugadorHealth.TakeDamage(burn);
                MostrarDaño(burn, playerBurnPoint, false, true);
                SpawnBurnParticles(playerBurnPoint);
                GameEvents.OnLifeChanged.Invoke(jugadorHealth);
            }
        }

        GameEvents.OnTurnChanged.Invoke(true);
        MostrarCartaActual();
        SetCartasAlpha(1f);

        esperandoSeleccion = true;
        cartasGroup.interactable = true;
        cartasGroup.blocksRaycasts = true;

        float tiempoTurno = 10f;
        while (tiempoTurno > 0f && esperandoSeleccion)
        {
            if (tutorialManager != null && tutorialManager.tutorialActivo)
                tiempoTurno = 10f;

            tiempoTurno -= Time.deltaTime;
            yield return null;
        }

        if (esperandoSeleccion)
        {
            esperandoSeleccion = false;
            StartCoroutine(TurnoEnemigoCoroutine());
        }
    }

    private void SpawnBurnParticles(Transform punto)
    {
        if (burnTickParticlesPrefab == null || punto == null) return;

        GameObject fx = Instantiate(burnTickParticlesPrefab, punto.position, Quaternion.identity);
        Destroy(fx, 3f);
    }

    private void EnemigoMuerto()
    {
        if (combatUI != null) combatUI.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (victoryText != null) victoryText.gameObject.SetActive(true);

        int exp = enemigo.ExpReward;
        PlayerExperience.Instance.AddExperience(exp);

        if (victoryExpBar != null)
        {
            victoryExpBar.gameObject.SetActive(false);
            victoryExpBar.gameObject.SetActive(true);
        }

        VictoryCamera.Instance.MoveCameraToVictory(jugadorHealth.transform);

        if (enemigoInstanciado != null)
            Destroy(enemigoInstanciado);

        StartCoroutine(VolverAlMundoTrasVictoria());
    }

    private IEnumerator VolverAlMundoTrasVictoria()
    {
        yield return new WaitForSeconds(5f);

        SceneManager.sceneLoaded += RestaurarPosicionJugador;
        SceneManager.LoadScene("SampleScene");
    }

    private void JugadorMuerto()
    {
        deathScreen.ShowDeathScreen();
        StartCoroutine(VolverAlMundo());
    }

    private IEnumerator VolverAlMundo()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.sceneLoaded += RestaurarPosicionJugador;
        SceneManager.LoadScene("SampleScene");
    }

    private void RestaurarPosicionJugador(Scene scene, LoadSceneMode mode)
    {
        if (PlayerPositionMemory.hasSavedPosition)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = PlayerPositionMemory.lastPosition;
            player.transform.rotation = PlayerPositionMemory.lastRotation;
        }

        SceneManager.sceneLoaded -= RestaurarPosicionJugador;
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

    public void MostrarMensaje(string texto)
    {
        StartCoroutine(MostrarMensajeCoroutine(texto));
    }

    private IEnumerator MostrarMensajeCoroutine(string texto)
    {
        if (failText == null)
            yield break;

        failText.text = texto;
        failText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.2f);

        failText.gameObject.SetActive(false);
    }

    public void MostrarDaño(int cantidad, Transform objetivo, bool esCuracion, bool esQuemadura = false)
    {
        if (objetivo == null) return;

        GameObject prefab = esQuemadura ? burnPopupPrefab : damagePopupPrefab;

        GameObject popup = Instantiate(prefab, canvas.transform);
        popup.transform.position = objetivo.position;

        popup.GetComponent<DamagePopup>().Setup(cantidad, esCuracion, esQuemadura);
    }

    public void MostrarEscudo(Transform objetivo)
    {
        if (shieldActivo != null) return;

        shieldActivo = Instantiate(shieldOrbitPrefab);

        ShieldOrbit orbit = shieldActivo.GetComponent<ShieldOrbit>();
        orbit.target = objetivo;
    }

    public void QuitarEscudo()
    {
        if (shieldActivo != null)
        {
            Destroy(shieldActivo);
            shieldActivo = null;
        }
    }

    private void ReducirTurnosDefensa()
    {
        if (defensaActiva)
        {
            defensaTurnosRestantes--;

            if (defensaTurnosRestantes <= 0)
            {
                defensaActiva = false;
                QuitarEscudo();
            }
        }
    }

    public void BloquearCartas()
    {
        esperandoSeleccion = false;
        cartasGroup.interactable = false;
        cartasGroup.blocksRaycasts = false;
    }

    public void DesbloquearCartas()
    {
        esperandoSeleccion = true;
        cartasGroup.interactable = true;
        cartasGroup.blocksRaycasts = true;
    }

    public void SetCartaPermitida(string nombreCarta)
    {
        cartaPermitida = nombreCarta;
    }
}