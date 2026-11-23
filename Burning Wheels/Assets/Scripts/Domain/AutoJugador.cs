// Oscar Alexander Vilchis Soto
// Script para el jugador para mover su monoplaza
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoJugador : MonoBehaviour
{
    public float velocidadMax = 400f;     // equivale a 250 km/h
    public float aceleracion = 8f;
    public float velocidadGiro = 95f;
    public float suavizadoVolante = 5f;  // qué tan rápido responden las teclas < >

    private Rigidbody rb;
    private float velocidadActual = 0f;
    private float giroActual = 0f;
    private float inputAvance;
    private float inputGiro;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()   // cada frame va a  leer el teclado
    {
        inputAvance = Input.GetAxisRaw("Vertical");
        inputGiro = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()   // física que ayuda amover con el Rigidbody
    {
        // Aceleración de 0 a 250km/H
        float velocidadObjetivo = inputAvance * velocidadMax;
        velocidadActual = Mathf.MoveTowards(velocidadActual, velocidadObjetivo, aceleracion * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + transform.forward * velocidadActual * Time.fixedDeltaTime);

        // Giro del carro 
        giroActual = Mathf.MoveTowards(giroActual, inputGiro, suavizadoVolante * Time.fixedDeltaTime);

        if (Mathf.Abs(velocidadActual) > 0.5f)
        {
            // Invierte el giro  en reversa 
            float direccion = velocidadActual >= 0f ? 1f : -1f;
            float factorVelocidad = Mathf.Clamp01(Mathf.Abs(velocidadActual) / 8f);
            float angulo = giroActual * velocidadGiro * direccion * factorVelocidad * Time.fixedDeltaTime;

            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, angulo, 0f));
        }
    }

    public float ObtenerVelocidadKmh()
    {
        return Mathf.Abs(velocidadActual) * 10f;
    }
}