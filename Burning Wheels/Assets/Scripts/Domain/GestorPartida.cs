// Oscar Alexander Vilchis Soto
// Script para gestionar la partda en general ( contador de vueltas, tiempo, etc)
// AI disclosure, durante la mplementación se usó ayuda de la IA para comprender y poder adaptar el como s emanejan las pantallas de recargar la escena de la clase practica a mi juego 
//  sin embargo el uso fue exclusivo para mayor comprensión de que usar y cómo adaptarlo y corregir unos errorcillos jeje.
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Administra el flujo de la partida, el cronómetro, el HUD y las pantallas de inicio, victoria y derrota
public class GestorPartida : MonoBehaviour
{
    [Header("Configuración de Carrera")]
    public float tiempoLimite = 60f;
    public AutoJugador autoJugador;

    [Header("HUD en Juego")]
    public TMP_Text textoVelocidad;
    public TMP_Text textoVueltas;
    public TMP_Text textoTiempo;

    [Header("Pantallas de Menú")]
    public GameObject panelInicio;        // panel inicial con el botón Jugar
    public GameObject panelVictoria;      // panel final con el botón Jugar de nuevo
    public TMP_Text textoVictoria;        // texto dentro de PanelVictoria para mostrar el tiempo
    public GameObject panelDerrota;       // panel final cuando se agota el tiempo

    private float tiempoRestante;
    private float tiempoTranscurrido = 0f;
    private bool juegoIniciado = false;
    private bool finCarrera = false;

    // Prepara el estado inicial de la partida y muestra el menú de inicio
    void Start()
    {
        tiempoRestante = tiempoLimite;
        juegoIniciado = false;
        finCarrera = false;

        if (autoJugador != null) autoJugador.puedeMoverse = false;
        if (panelInicio != null) panelInicio.SetActive(true);
        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (panelDerrota != null) panelDerrota.SetActive(false);

        if (textoVueltas != null) textoVueltas.text = "Meta: Cruza antes de 60s";
        if (textoTiempo != null) textoTiempo.text = "Tiempo: " + tiempoLimite.ToString("F1") + " s";
    }

    // Inicia la carrera al presionar el botón Jugar en el menú principal
    public void IniciarJuego()
    {
        juegoIniciado = true;
        if (panelInicio != null) panelInicio.SetActive(false);
        if (autoJugador != null) autoJugador.puedeMoverse = true;
    }

    // Actualiza el velocímetro y el cronómetro mientras la carrera está activa
    void Update()
    {
        if (autoJugador != null && textoVelocidad != null)
        {
            int velocidad = (int)autoJugador.ObtenerVelocidadKmh();
            textoVelocidad.text = velocidad + " km/h";
        }

        if (!juegoIniciado || finCarrera) return;

        tiempoTranscurrido += Time.deltaTime;
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            finCarrera = true;
            if (autoJugador != null) autoJugador.puedeMoverse = false;

            textoTiempo.text = "Tiempo: 0.0 s";
            textoVueltas.text = "¡Se acabó el tiempo!";

            // Muestra la pantalla de derrota
            if (panelDerrota != null) panelDerrota.SetActive(true);
        }
        else
        {
            textoTiempo.text = "Tiempo: " + tiempoRestante.ToString("F1") + " s";
        }
    }

    // Detecta cuando el jugador cruza la línea de meta y muestra la pantalla de victoria
    void OnTriggerEnter(Collider other)
    {
        // Evita activar la meta al segundo cero si el auto arranca cerca de la línea de meta
        if (!juegoIniciado || finCarrera || tiempoTranscurrido < 3f) return;

        if (other.CompareTag("Jugador") || other.transform.root.CompareTag("Jugador"))
        {
            finCarrera = true;
            if (autoJugador != null) autoJugador.puedeMoverse = false;

            float tiempoTardado = tiempoLimite - tiempoRestante;
            textoVueltas.text = "¡Ganaste en " + tiempoTardado.ToString("F1") + " s!";

            if (panelVictoria != null)
            {
                panelVictoria.SetActive(true);
                if (textoVictoria != null)
                {
                    textoVictoria.text = "¡GANASTE!\nTiempo: " + tiempoTardado.ToString("F1") + " s";
                }
            }
        }
    }

    // Recarga la escena actual para jugar una nueva partida
    public void ReiniciarCarrera()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}