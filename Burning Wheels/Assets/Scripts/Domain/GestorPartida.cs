// Oscar Alexander Vilchis Soto
// Script para gestionar la partda en general ( contador de vueltas, tiempo, etc)

using UnityEngine;
using TMPro;

public class GestorPartida : MonoBehaviour
{
    public int vueltasTotales = 3;
    public AutoJugador autoJugador;

    public TMP_Text textoVelocidad;
    public TMP_Text textoVueltas;
    public TMP_Text textoTiempo;

    private int vueltaActual = 1;
    private float tiempo = 0f;
    private bool finCarrera = false;

    void Start()
    {
        textoVueltas.text = "Vuelta: " + vueltaActual + " / " + vueltasTotales;
    }

    void Update()
    {
        if (finCarrera == false)
        {
            tiempo = tiempo + Time.deltaTime;
            textoTiempo.text = "Tiempo: " + tiempo.ToString("F1") + " s";
        }

        int velocidad = (int)autoJugador.ObtenerVelocidadKmh();
        textoVelocidad.text = velocidad + " km/h";
    }

    void OnTriggerEnter(Collider other)
    {
        if (finCarrera == true) return;

        if (other.CompareTag("Jugador") || other.transform.root.CompareTag("Jugador"))
        {
            if (vueltaActual < vueltasTotales)
            {
                vueltaActual++;
                textoVueltas.text = "Vuelta: " + vueltaActual + " / " + vueltasTotales;
            }
            else
            {
                finCarrera = true;
                textoVueltas.text = "¡Carrera Terminada!";
            }
        }
    }
}