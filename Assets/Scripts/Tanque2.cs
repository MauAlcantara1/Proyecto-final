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
    [SerializeField] private float duracionCarga = 1f;
    [SerializeField] private float duracionDisparo = 3f;
    [SerializeField] private float pausaEntreCiclos = 4.5f;

    [Header("Ataque Secundario (Embestida)")]
    [SerializeField] private float fuerzaEmbestida = 8f;
    [SerializeField] private float distanciaRecorridoEmbestida = 7f;
    [SerializeField] private float pausaPostEmbestida = 2f;

    [Header("Orientación del Sprite")]
    [SerializeField] private bool miraDerechaPorDefecto = false;
    private bool estaMuerto = false;
    private int vidaActual;

    [SerializeField] private GameObject prefabBala;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float fuerzaDisparo = 8f;

    // 🟩🟩🟩 AUDIO DE MUERTE (NUEVO)
    [Header("Audio")]
    [SerializeField] private AudioClip sonidoMuerte;
    private AudioSource audioSource;
    // 🟩🟩🟩 FIN

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
        VoltearTanque(mirandoDerecha);

        if (rb != null)
            rb.freezeRotation = true;

        vidaActual = vidaMaxima;

        // 🟩 OBTENER AUDIOSOURCE (NUEVO)
        audioSource = GetComponent<AudioSource>();
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

    // ========== CICLO ATAQUE ==========

    private IEnumerator CicloAtaque()
    {
        enCicloAtaque = true;
        atacando = true;

        while (jugadorDetectado && !embistiendo)
        {
            anim.SetBool("Cargar", true);
            anim.SetBool("Disparar", false);
            yield return new WaitForSeconds(duracionCarga);

            anim.SetBool("Cargar", false);
            anim.SetBool("Disparar", true);
            yield return new WaitForSeconds(duracionDisparo);

            anim.SetBool("Disparar", false);
            yield return new WaitForSeconds(pausaEntreCiclos);

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

        anim.SetBool("Cargar", false);
        anim.SetBool("Volver a disp", false);
        anim.SetBool("Girar", false);

        embistiendo = true;
        atacando = false;
        enCicloAtaque = false;
        puedeEmbestir = false;

        anim.SetBool("Embestir", true);
        yield return null;
        anim.SetBool("Embestir", false);

        yield return new WaitForSeconds(0.3f);

        Vector3 direccion = mirandoDerecha ? Vector3.right : Vector3.left;
        float distanciaRecorrida = 0f;
        Vector3 inicio = transform.position;

        while (distanciaRecorrida < distanciaRecorridoEmbestida)
        {
            transform.Translate(direccion * fuerzaEmbestida * Time.deltaTime, Space.World);
            distanciaRecorrida = Vector3.Distance(inicio, transform.position);
            yield return null;
        }

        if (pausaPostEmbestida > 0)
            yield return new WaitForSeconds(pausaPostEmbestida);

        StartCoroutine(GiroTrasEmbestida());
    }

    private IEnumerator GiroTrasEmbestida()
    {
        anim.SetBool("Girar", true);
        yield return new WaitForSeconds(1.660f);

        anim.SetBool("Girar", false);
        VoltearTanque(!mirandoDerecha);

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
            VoltearTanque(debeMirarDerecha);
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
        mirandoDerecha = mirarDerechaNuevo;

        Vector3 escala = transform.localScale;
        escala.x = mirandoDerecha ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
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

        // 🟩 REPRODUCIR SONIDO DE MUERTE (NUEVO)
        if (audioSource != null && sonidoMuerte != null)
            audioSource.PlayOneShot(sonidoMuerte);
        // 🟩 FIN

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // ← NUEVO
        }


        if (colisionesTanque != null)
            colisionesTanque.enabled = false;

        anim.Play("Muerte");
        anim.SetTrigger("Muerte");

        StopAllCoroutines();

        GameObject[] balas = GameObject.FindGameObjectsWithTag("bala");
        foreach (GameObject bala in balas)
        {
            Collider2D colBala = bala.GetComponent<Collider2D>();
            if (colBala != null && colisionesTanque != null)
                Physics2D.IgnoreCollision(colisionesTanque, colBala, true);
        }

        DropLoot drop = GetComponent<DropLoot>();
        if (drop != null)
        {
            drop.SoltarObjetos();
            Debug.Log("🎲 Drop ejecutado al morir el tanque.");
        }
    }

    private void Disparar()
    {
        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, puntoDisparo.rotation);
        Rigidbody2D rbBala = bala.GetComponent<Rigidbody2D>();

        if (rbBala != null)
        {
            float direccionX = Mathf.Sign(transform.localScale.x);
            rbBala.linearVelocity = new Vector2(direccionX * fuerzaDisparo, 0f);
        }
    }
}
