using System.Collections;
using UnityEngine;

public class Tanque2 : MonoBehaviour
{
    [Header("Detección y Movimiento")]
    [SerializeField] private float distanciaDeteccion = 25f;
    [SerializeField] private float distanciaParada = 10f;
    [SerializeField] private float velocidadMovimiento = 1.5f;

    [Header("Ataque Principal (Carga y Disparo)")]
    [SerializeField] private float retrasoAntesDeAtacar = 4f;
    [SerializeField] private float duracionCarga = 4f;
    [SerializeField] private float duracionDisparo = 3f;
    [SerializeField] private float pausaEntreCiclos = 4.5f;

    [Header("Ataque Secundario (Embestida)")]
    [SerializeField] private float distanciaEmbestida = 10f;       // Distancia para activar embestida
    [SerializeField] private float fuerzaEmbestida = 8f;           // Velocidad del impulso
    [SerializeField] private float distanciaRecorridoEmbestida = 20f;
    [SerializeField] private float pausaPostEmbestida = 2f;

    [Header("Orientación del Sprite")]
    [SerializeField] private bool miraDerechaPorDefecto = true;

    private Transform jugador;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D[] colisionesTanque;
    private Collider2D[] colJugador;

    private bool jugadorDetectado = false;
    private bool enMovimiento = false;
    private bool mirandoDerecha;
    private bool atacando = false;
    private bool enCicloAtaque = false;
    private bool embistiendo = false;
    private bool girando = false;

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colisionesTanque = GetComponentsInChildren<Collider2D>();

        if (jugador != null)
            colJugador = jugador.GetComponentsInChildren<Collider2D>();

        mirandoDerecha = miraDerechaPorDefecto;
        if (rb != null)
            rb.freezeRotation = true;

        // ✅ Corrige orientación inicial del sprite
        spriteRenderer.flipX = mirandoDerecha ? false : true;

        Debug.Log($"✅ Tanque2 iniciado mirando {(mirandoDerecha ? "a la derecha" : "a la izquierda")} en Idle (esperando jugador).");
    }

    private void Update()
    {
        if (jugador == null || atacando || embistiendo || girando) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // 🔶 Detecta jugador
        if (!jugadorDetectado && distancia <= distanciaDeteccion)
        {
            jugadorDetectado = true;
            anim.SetTrigger("DetEnem");
            enMovimiento = true;
            Debug.Log("🎯 Jugador detectado → Inicia movimiento hacia él.");
        }

        // 🔶 Movimiento
        if (enMovimiento)
        {
            if (distancia > distanciaParada)
            {
                MoverHaciaJugador();
            }
            else
            {
                DetenerMovimiento();
                enMovimiento = false;
                anim.SetTrigger("DecidirAtk");
                Debug.Log("🛑 Tanque2 se detiene → Idle 0 (DecidirAtk).");

                if (!enCicloAtaque)
                    Invoke(nameof(IniciarAtaque), retrasoAntesDeAtacar);
            }
        }

        // 🔶 Si el jugador se acerca demasiado → Embestir
        if (!embistiendo && distancia <= distanciaEmbestida)
        {
            CancelInvoke(nameof(IniciarAtaque)); // cancela cualquier otro ataque
            StartCoroutine(Embestida());
        }

        // 🔶 Si el jugador se aleja
        else if (jugadorDetectado && distancia > distanciaDeteccion + 5f)
        {
            jugadorDetectado = false;
            anim.ResetTrigger("DetEnem");
            anim.ResetTrigger("DecidirAtk");
            CancelInvoke(nameof(IniciarAtaque));
            enCicloAtaque = false;
            atacando = false;
            embistiendo = false;
            girando = false;
            Debug.Log("🚶‍♂️ Jugador fuera de rango → vuelve a Idle.");
        }
    }

    private void IniciarAtaque()
    {
        if (enCicloAtaque || embistiendo) return;
        enCicloAtaque = true;
        atacando = true;
        StartCoroutine(CicloAtaque());
    }

    private IEnumerator CicloAtaque()
    {
        while (jugadorDetectado && !embistiendo)
        {
            anim.SetTrigger("Cargar");
            Debug.Log("⚡ Tanque2 comienza CARGA...");
            yield return new WaitForSeconds(duracionCarga);

            anim.SetTrigger("DispEnem");
            Debug.Log("💥 Tanque2 dispara...");
            yield return new WaitForSeconds(duracionDisparo);

            anim.SetTrigger("Volver a disp");
            Debug.Log("🔁 Tanque2 vuelve a CARGAR (Volver a disp).");
            yield return new WaitForSeconds(pausaEntreCiclos);
        }

        atacando = false;
        enCicloAtaque = false;
    }

    private IEnumerator Embestida()
    {
        embistiendo = true;
        atacando = true;
        enCicloAtaque = false;
        DetenerMovimiento();

        anim.SetTrigger("Embestir");
        Debug.Log("🚀 Embestida iniciada.");

        // 🔹 Ignorar colisiones con jugador
        if (colJugador != null)
        {
            foreach (var cTan in colisionesTanque)
                foreach (var cJug in colJugador)
                    Physics2D.IgnoreCollision(cTan, cJug, true);
        }

        yield return new WaitForSeconds(0.3f); // pequeña pausa antes del impulso

        Vector3 direccion = (jugador.position - transform.position).normalized;
        float distanciaRecorrida = 0f;
        Vector3 inicio = transform.position;

        // 🔹 Movimiento hacia adelante
        while (distanciaRecorrida < distanciaRecorridoEmbestida)
        {
            transform.Translate(direccion * fuerzaEmbestida * Time.deltaTime, Space.World);
            distanciaRecorrida = Vector3.Distance(inicio, transform.position);
            yield return null;
        }

        Debug.Log("💥 Tanque2 impacta al jugador (daño aplicado)");

        // 🔹 Reactivar colisiones
        if (colJugador != null)
        {
            foreach (var cTan in colisionesTanque)
                foreach (var cJug in colJugador)
                    Physics2D.IgnoreCollision(cTan, cJug, false);
        }

        embistiendo = false;
        atacando = false;

        // 🔁 Inicia animación de giro al terminar la embestida
        StartCoroutine(GiroTrasEmbestida());
    }

    private IEnumerator GiroTrasEmbestida()
    {
        girando = true;
        anim.SetTrigger("Girar");
        Debug.Log("🔄 Inicia animación de giro...");

        // Espera a que termine la animación de giro (ajusta duración según tu clip)
        yield return new WaitForSeconds(1.2f);

        // ✅ Cambia la dirección visual del sprite
        mirandoDerecha = !mirandoDerecha;
        spriteRenderer.flipX = !spriteRenderer.flipX;
        Debug.Log($"↩️ Dirección cambiada. Ahora mirando {(mirandoDerecha ? "a la derecha" : "a la izquierda")}");

        yield return new WaitForSeconds(0.3f);

        // 🔁 Luego del giro → vuelve al ciclo de ataques
        anim.SetTrigger("Cargar");
        Debug.Log("⚡ Vuelve a cargar tras el giro.");

        girando = false;
        atacando = true;
        StartCoroutine(CicloAtaque());
    }

    private void MoverHaciaJugador()
    {
        Vector2 direccion = (jugador.position - transform.position).normalized;
        bool debeMirarDerecha = direccion.x > 0;

        if (debeMirarDerecha != mirandoDerecha)
        {
            mirandoDerecha = debeMirarDerecha;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        transform.position += (Vector3)(direccion * velocidadMovimiento * Time.deltaTime);
    }

    private void DetenerMovimiento()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaParada);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaEmbestida);
    }
}
