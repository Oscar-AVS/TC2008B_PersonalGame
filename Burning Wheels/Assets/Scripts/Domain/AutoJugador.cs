// Oscar Alexander Vilchis Soto
// Script para el jugador para mover su monoplaza
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

    /// Lee las entradas del teclado en cada frame
    void Update()
    {
        inputAvance = Input.GetAxisRaw("Vertical");
        inputGiro = Input.GetAxisRaw("Horizontal");
    }

    /// Ejecuta el movimiento físico y actualiza los efectos
    void FixedUpdate()
    {
        ActualizarVelocidad();
        ActualizarGiro();
        ActualizarDerrape();
    }

    private void ActualizarVelocidad()
    {
        float ritmo = (inputAvance != 0f) ? aceleracion : desaceleracion;
        velocidadActual = Mathf.MoveTowards(velocidadActual, inputAvance * velocidadMax, ritmo * Time.fixedDeltaTime);

        Vector3 movimiento = transform.forward * velocidadActual;
        rb.linearVelocity = new Vector3(movimiento.x, rb.linearVelocity.y, movimiento.z);
    }

    private void ActualizarGiro()
    {
        giroActual = Mathf.MoveTowards(giroActual, inputGiro, suavizadoVolante * Time.fixedDeltaTime);

        if (Mathf.Abs(velocidadActual) <= 0.5f) return;

        float factorVelocidad = Mathf.Clamp01(Mathf.Abs(velocidadActual) / 8f) * Mathf.Sign(velocidadActual);
        float angulo = giroActual * velocidadGiro * factorVelocidad * Time.fixedDeltaTime;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, angulo, 0f));
    }

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

    /// Regresa el monoplaza a la línea de salida con velocidad en cero
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            RegresarAlInicio();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstaculo"))
        {
            Debug.Log("¡Tocado obstáculo! Regresando al inicio...");
            RegresarAlInicio();
        }
    }

    public float ObtenerVelocidadKmh() => Mathf.Abs(velocidadActual) * 10f;
}