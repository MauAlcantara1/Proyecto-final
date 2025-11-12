using System.Collections;
using UnityEngine;

public class Tanque2 : MonoBehaviour
{
    [Header("Rangos y Movimiento")]
    [SerializeField] private float distanciaDeteccion = 25f;
    [SerializeField] private float distanciaParada = 10f;
    [SerializeField] private float distanciaEmbestida = 3f;
    [SerializeField] private float velocidadMovimiento = 1.5f;

    [Header("Ataque Principal (Carga y Disparo)")]
    [SerializeField] private float retrasoAntesDeAtacar = 4f;
    [SerializeField] private float duracionCarga = 1f;
    [SerializeField] private float duracionDisparo = 3f;
    [SerializeField] private float pausaEntreCiclos = 4.5f;
    [SerializeField] private Transform puntoDisparo;

    [Header("Ataque Secundario (Embestida)")]
    [SerializeField] private float fuerzaEmbestida = 8f;
    [SerializeField] private float distanciaRecorridoEmbestida = 7f;
    [SerializeField] private float pausaPostEmbestida = 2f;

    [Header("Orientación del Sprite")]
    [SerializeField] private bool miraDerechaPorDefecto = false;
    private bool estaMuerto = false;
    private int vidaActual;



    // Componentes y estados
    private Transform jugador;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisionesTanque;
    private Collider2D[] colJugador;

    // Estados lógicos
    private bool jugadorDetectado = false;
    private bool enRangoAtaque = false;
    private bool enRangoEmbestida = false;
    private bool enMovimiento = false;
    private bool mirandoDerecha;
    private bool atacando = false;
    private bool enCicloAtaque = false;
    private bool embistiendo = false;
    private bool girando = false;
    private bool puedeEmbestir = true;

    [SerializeField] private int vidaMaxima = 10;

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colisionesTanque = GetComponent<Collider2D>();

        if (jugador != null)
            colJugador = jugador.GetComponentsInChildren<Collider2D>();

        mirandoDerecha = miraDerechaPorDefecto;
        spriteRenderer.flipX = !mirandoDerecha;

        if (rb != null)
            rb.freezeRotation = true;
            
        vidaActual = vidaMaxima;

    }

    private void Update()
    {
        if (jugador == null || embistiendo || girando || estaMuerto) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        jugadorDetectado = distancia <= distanciaDeteccion;
        enRangoAtaque = jugadorDetectado && distancia <= distanciaParada && distancia > distanciaEmbestida;
        enRangoEmbestida = jugadorDetectado && distancia <= distanciaEmbestida;

        if (!jugadorDetectado)
        {
            ResetEstados();
            return;
        }

        if (enRangoEmbestida && puedeEmbestir && !embistiendo)
        {
            StartCoroutine(Embestida());
            return;
        }

        if (enRangoAtaque && !atacando && !enCicloAtaque)
        {
            StartCoroutine(CicloAtaque());
            return;
        }

        if (jugadorDetectado && distancia > distanciaParada && !atacando && !embistiendo)
        {
            MoverHaciaJugador();
        }
    }

    private void ResetEstados()
    {
        enMovimiento = false;
        atacando = false;
        enCicloAtaque = false;
        embistiendo = false;
        girando = false;
        puedeEmbestir = true;
    }

    // ========== CICLO DE ATAQUE ==========
    private IEnumerator CicloAtaque()
    {
        enCicloAtaque = true;
        atacando = true;

        while (jugadorDetectado && !embistiendo)
        {
            // 🔹 Etapa 1: CARGA
            anim.SetBool("Cargar", true);
            anim.SetBool("Disparar", false);
            yield return new WaitForSeconds(duracionCarga);

            // 🔹 Etapa 2: DISPARO
            anim.SetBool("Cargar", false);
            anim.SetBool("Disparar", true);
            yield return new WaitForSeconds(duracionDisparo);

            // 🔹 Etapa 3: Reinicio del ciclo
            anim.SetBool("Disparar", false);
            yield return new WaitForSeconds(pausaEntreCiclos);

            // Si el jugador se acercó demasiado, embestir
            float distancia = Vector2.Distance(transform.position, jugador.position);
            if (puedeEmbestir && distancia <= distanciaEmbestida)
            {
                anim.SetBool("Cargar", false);
                anim.SetBool("Disparar", false);
                enCicloAtaque = false;
                atacando = false;
                StartCoroutine(Embestida());
                yield break;
            }
        }

        enCicloAtaque = false;
        atacando = false;
    }



    // ========== EMBESTIDA ==========
   private IEnumerator Embestida()
    {
        if (embistiendo) yield break;

        // Limpieza
        anim.SetBool("Cargar", false);
        anim.SetBool("Volver a disp", false);
        anim.SetBool("Girar", false);

        embistiendo = true;
        atacando = false;
        enCicloAtaque = false;
        puedeEmbestir = false;

        // 🔹 Animación de embestida
        anim.SetBool("Embestir", true);
        yield return null;
        anim.SetBool("Embestir", false);

        yield return new WaitForSeconds(0.3f);

        // Movimiento de embestida
        Vector3 direccion = mirandoDerecha ? Vector3.right : Vector3.left;
        float distanciaRecorrida = 0f;
        Vector3 inicio = transform.position;

        while (distanciaRecorrida < distanciaRecorridoEmbestida)
        {
            transform.Translate(direccion * fuerzaEmbestida * Time.deltaTime, Space.World);
            distanciaRecorrida = Vector3.Distance(inicio, transform.position);
            yield return null;
        }

        // Si pausaPostEmbestida > 0, esperamos; si no, giramos enseguida
        if (pausaPostEmbestida > 0)
            yield return new WaitForSeconds(pausaPostEmbestida);

        // 🔹 Inicia el giro inmediatamente
        StartCoroutine(GiroTrasEmbestida());

    }

    private IEnumerator GiroTrasEmbestida()
    {
        anim.SetBool("Girar", true);
        yield return new WaitForSeconds(1.660f);

        anim.SetBool("Girar", false);
        VoltearTanque(!mirandoDerecha);
        mirandoDerecha = !mirandoDerecha;


        embistiendo = false;
        atacando = false;
        puedeEmbestir = true;

        StartCoroutine(CicloAtaque());
    }


    // ========== MOVIMIENTO ==========
    private void MoverHaciaJugador()
    {
        if (jugador == null) return;

        Vector2 dir = (jugador.position - transform.position).normalized;
        bool debeMirarDerecha = dir.x > 0;

        if (debeMirarDerecha != mirandoDerecha)
        {
            mirandoDerecha = debeMirarDerecha;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        transform.position += (Vector3)(dir * velocidadMovimiento * Time.deltaTime);
    }

    private void DetenerMovimiento()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, distanciaParada);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, distanciaEmbestida);
    }

    private void VoltearTanque(bool mirarDerechaNuevo)
    {
        Vector3 escala = transform.localScale;
        escala.x = mirarDerechaNuevo ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;

        Vector3 pos = puntoDisparo.localPosition;
        pos.x = -pos.x;
        puntoDisparo.localPosition = pos;
    }

    public void RecibirDaño(int cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;
        Debug.Log($"💥 Tanque recibió {cantidad} de daño. Vida restante: {vidaActual}");

        if (vidaActual <= 0) Morir();
    }

    private void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        anim.Play("Muerte");

        anim.SetTrigger("Muerte");

        DropLoot drop = GetComponent<DropLoot>();
        if (drop != null)
        {
            drop.SoltarObjetos();
            Debug.Log("🎲 Drop ejecutado al morir el tanque.");
        }
    }

}
