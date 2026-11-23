// Oscar Alexander Vilchis Soto
// Script para el jugador para mover su monoplaza
// AI disclosure, durante la mplementación se usó ayuda de la IA para comprender y pode rimplementar clauclos
// de física para la aceleración y desaceleración, así como el giro del carro, sin embargo el uso fue exclusivo de 
// comprensión de que usar y cómo implementarlo.

using UnityEngine;

/// Controla el movimiento físico, el efecto visual de derrape y el reinicio de posición del monoplaza
[RequireComponent(typeof(Rigidbody))]
public class AutoJugador : MonoBehaviour
{
    [Header("Parámetros de Movimiento")]
    public float velocidadMax = 40f;      // equivale a 400 km/h
    public float aceleracion = 12f;
    public float desaceleracion = 3f;
    public float velocidadGiro = 95f;
    public float suavizadoVolante = 5f;
    public Transform puntoInicio;         // punto al que regresa si toca algo fuera de pista

    [Header("Visuales y Derrape")]
    public Transform modeloAuto;          // hijo visual (modelo Lauda) separado de la física
    public float anguloDerrape = 18f;
    public TrailRenderer[] marcasLlantas; // marcas en el piso

    [HideInInspector]
    public bool puedeMoverse = false;     // bloquea el movimiento en pantallas de menú o victoria

    private Rigidbody rb;
    private float velocidadActual;
    private float giroActual;
    private float inputAvance;
    private float inputGiro;
    private Quaternion rotacionInicialModelo;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // evita que se voltee al chocar 

        if (modeloAuto != null)
            rotacionInicialModelo = modeloAuto.localRotation;
    }

    // Lee las entradas del teclado en cada frame
    void Update()
    {
        // Si la carrera no ha iniciado o ya terminó, ignora el teclado
        if (!puedeMoverse)
        {
            inputAvance = 0f;
            inputGiro = 0f;
            return;
        }

        inputAvance = Input.GetAxisRaw("Vertical");
        inputGiro = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        // Mantiene el auto detenido en horizontal si no tiene permiso de moverse
        if (!puedeMoverse)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        ActualizarVelocidad();
        ActualizarGiro();
        ActualizarDerrape();
    }

    // Calcula la aceleración y desaceleración paulatina hacia adelante o en reversa
    private void ActualizarVelocidad()
    {
        float ritmo = (inputAvance != 0f) ? aceleracion : desaceleracion;
        velocidadActual = Mathf.MoveTowards(velocidadActual, inputAvance * velocidadMax, ritmo * Time.fixedDeltaTime);

        Vector3 movimiento = transform.forward * velocidadActual;
        rb.linearVelocity = new Vector3(movimiento.x, rb.linearVelocity.y, movimiento.z);
    }

    // Aplica rotación  proporcional a la velocidad del carrito
    private void ActualizarGiro()
    {
        giroActual = Mathf.MoveTowards(giroActual, inputGiro, suavizadoVolante * Time.fixedDeltaTime);

        if (Mathf.Abs(velocidadActual) <= 0.5f) return;

        float factorVelocidad = Mathf.Clamp01(Mathf.Abs(velocidadActual) / 8f) * Mathf.Sign(velocidadActual);
        float angulo = giroActual * velocidadGiro * factorVelocidad * Time.fixedDeltaTime;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, angulo, 0f));
    }

    // Inclina el modelo 3D y activa las marcasd de las llantas al driftear (i wonder if u know how they live in tokyo)
    private void ActualizarDerrape()
    {
        bool estaDerrapando = (inputAvance < 0f && velocidadActual > 8f && Mathf.Abs(inputGiro) > 0.1f);

        if (modeloAuto != null)
        {
            float cruce = estaDerrapando ? (giroActual * anguloDerrape) : 0f;
            Quaternion objetivo = rotacionInicialModelo * Quaternion.Euler(0f, 0f, cruce);
            modeloAuto.localRotation = Quaternion.Lerp(modeloAuto.localRotation, objetivo, 8f * Time.fixedDeltaTime);
        }

        foreach (TrailRenderer marca in marcasLlantas)
        {
            if (marca != null) marca.emitting = estaDerrapando;
        }
    }

    // Regresa el monoplaza a la línea de salida con velocidad en cero
    public void RegresarAlInicio()
    {
        velocidadActual = 0f;
        giroActual = 0f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (puntoInicio != null)
        {
            rb.position = puntoInicio.position;
            rb.rotation = puntoInicio.rotation;
        }
    }

    // Detecta colisiones sólidas contra vallas, muros o pasto fuera de pista
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            RegresarAlInicio();
        }
    }

    // Detecta triggers marcados como obstáculo
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstaculo"))
        {
            Debug.Log("¡Tocado obstáculo! Regresando al inicio...");
            RegresarAlInicio();
        }
    }

    // Convierte la velocidad interna a escala visual en km/h para el HUD
    public float ObtenerVelocidadKmh() => Mathf.Abs(velocidadActual) * 10f;
}