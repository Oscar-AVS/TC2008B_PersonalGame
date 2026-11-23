// Oscar Alexander Vilchis Soto
// Script para gestionar la partda en general ( contador de vueltas, tiempo, etc)
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GestorPartida : MonoBehaviour
{
    public float tiempoLimite = 60f;
    public AutoJugador autoJugador;

    public TMP_Text textoVelocidad;
    public TMP_Text textoVueltas;
    public TMP_Text textoTiempo;

    private float tiempoRestante;
    private bool finCarrera = false;

    void Start()
    {
        tiempoRestante = tiempoLimite;
        textoVueltas.text = "Meta: Cruza antes de 60s";
    }

    void Update()
    {
        if (autoJugador != null && textoVelocidad != null)
        {
            int velocidad = (int)autoJugador.ObtenerVelocidadKmh();
            textoVelocidad.text = velocidad + " km/h";
        }

        if (finCarrera) return;

        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            finCarrera = true;
            textoTiempo.text = "Tiempo: 0.0 s";
            textoVueltas.text = "¡Se acabó el tiempo!";
            Invoke("ReiniciarCarrera", 2f);
        }
        else
        {
            textoTiempo.text = "Tiempo: " + tiempoRestante.ToString("F1") + " s";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (finCarrera) return;

        if (other.CompareTag("Jugador") || other.transform.root.CompareTag("Jugador"))
        {
            finCarrera = true;
            float tiempoTardado = tiempoLimite - tiempoRestante;
            textoVueltas.text = "¡Ganaste en " + tiempoTardado.ToString("F1") + " s!";
        }
    }

    void ReiniciarCarrera()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}