// Oscar Alexander Vilchis Soto
// Script para gestionar la partda en general ( contador de vueltas, tiempo, etc)

using UnityEngine;

public class GestorPartida : MonoBehaviour
{
    public int vueltasTotales = 3;
    private int vueltaActual = 0;
    private bool carreraTerminada = false;

    void OnTriggerEnter(Collider otro)
    {
        if (carreraTerminada) return;

        // Detecta si quien cruzó el trigger es el coche (o su hijo con Collider)
        if (otro.CompareTag("Jugador") || otro.transform.root.CompareTag("Jugador"))
        {
            vueltaActual++;
            Debug.Log("Vuelta: " + vueltaActual + " / " + vueltasTotales);

            if (vueltaActual > vueltasTotales)
            {
                carreraTerminada = true;
                Debug.Log("¡Carrera terminada! ¡Ganaste!");
            }
        }
    }
}