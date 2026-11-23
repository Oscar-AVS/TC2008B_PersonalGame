using UnityEngine;

public class DetectorChoque : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            AutoJugador jugador = GetComponentInParent<AutoJugador>();
            if (jugador != null)
            {
                jugador.RegresarAlInicio();
            }
        }
    }
}