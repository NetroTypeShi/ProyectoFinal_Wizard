using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    public GameObject burnPopupPrefab;   //  PREFAB DE QUEMADURA
    public Canvas canvas;

    [Header("Damage Popup Targets")]
    public Transform enemyDamagePoint;
    public Transform playerDamagePoint;

    [Header("🔥 NUEVOS: puntos de quemadura")]
    public Transform enemyBurnPoint;
    public Transform playerBurnPoint;

    [Header("Shield Orbit")]
    public GameObject shieldOrbitPrefab;
    private GameObject shieldActivo;

    [Header("Efectos de curación")]
    public GameObject healParticlesPrefab;

    [Header("Efectos de quemadura")]
    public GameObject burnTickParticlesPrefab;

    private GameObject enemigoDelMundo;
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

        if (failText != null)
            failText.gameObject.SetActive(false);

        if (victoryText != null)
            victoryText.gameObject.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);
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
        enemigoDelMundo = enemigoGO; 
        enemigo = enemigoGO.GetComponent<Enemy>();
        enemigoHealth = enemigoGO.GetComponent<HealthComponent>();
        enemigoHealth.OnDeath += EnemigoMuerto;
        enemigoHealth.OnHealthChanged += (vidaActual, vidaMax) => 
        { 
            GameEvents.OnLifeChanged.Invoke(enemigoHealth); 
        };
        GameEvents.OnLifeChanged.Invoke(enemigoHealth);
        StatusEffects playerStatus = jugadorHealth.GetComponent<StatusEffects>();
        if (playerStatus != null) 
        {
            playerStatus.burnTurns = 0; playerStatus.burnDamage = 0;
        }
        StatusEffects enemyStatus = enemigoHealth.GetComponent<StatusEffects>(); 
        if (enemyStatus != null) 
        {
            enemyStatus.burnTurns = 0;
            enemyStatus.burnDamage = 0; 
        } 
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
        StatusEffects enemyStatus = enemigoHealth.GetComponent<StatusEffects>();
        if (enemyStatus != null && enemyStatus.HasBurn())
        {
            int burn = enemyStatus.TickBurn();
            if (burn > 0)
            {
                enemigoHealth.TakeDamage(burn);

                MostrarDaño(burn, enemyBurnPoint, false, true);

                GameEvents.OnLifeChanged.Invoke(enemigoHealth);

                if (burnTickParticlesPrefab != null)
                {
                    GameObject fx = Instantiate(burnTickParticlesPrefab, enemigoHealth.transform.position, Quaternion.identity);
                    Destroy(fx, 1f);
                }
            }
        }

        yield return new WaitForSeconds(1f);

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

        ReducirTurnosDefensa();

        TurnoJugador();
    }

    private void TurnoJugador()
    {
        StatusEffects playerStatus = jugadorHealth.GetComponent<StatusEffects>();
        if (playerStatus != null && playerStatus.HasBurn())
        {
            int burn = playerStatus.TickBurn();
            if (burn > 0)
            {
                jugadorHealth.TakeDamage(burn);

                MostrarDaño(burn, playerBurnPoint, false, true);

                GameEvents.OnLifeChanged.Invoke(jugadorHealth);

                if (burnTickParticlesPrefab != null)
                {
                    GameObject fx = Instantiate(burnTickParticlesPrefab, jugadorHealth.transform.position, Quaternion.identity);
                    Destroy(fx, 1f);
                }
            }
        }

        GameEvents.OnTurnChanged.Invoke(true);

        esperandoSeleccion = true;
        MostrarCartaActual();

        SetCartasAlpha(1f);
    }

    private void EnemigoMuerto()
    {
        Debug.Log("El enemigo ha muerto");

        if (combatUI != null)
            combatUI.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (victoryText != null)
            victoryText.gameObject.SetActive(true);

        int expGanada = enemigo.ExpReward;
        PlayerExperience.Instance.AddExperience(expGanada);

        if (victoryExpBar != null)
        {
            victoryExpBar.gameObject.SetActive(false);
            victoryExpBar.gameObject.SetActive(true);
        }

        VictoryCamera.Instance.MoveCameraToVictory(jugadorHealth.transform);

        EnemyStateManager.enemyDead = true;
        EnemyStateManager.respawnTime = Time.time + 10f;

        if (enemigo != null)
            Destroy(enemigo.gameObject);

        if (enemigoDelMundo != null)
            enemigoDelMundo.SetActive(false);

        StartCoroutine(VolverAlMundoTrasVictoria());
    }

    private IEnumerator VolverAlMundoTrasVictoria()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("SampleScene");
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

        GameObject prefabAUsar = esQuemadura ? burnPopupPrefab : damagePopupPrefab; 

        GameObject popup = Instantiate(prefabAUsar, canvas.transform);

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
}

