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

    [Header("Ataque Secundario (Embestida)")]
    [SerializeField] private float fuerzaEmbestida = 8f;
    [SerializeField] private float distanciaRecorridoEmbestida = 7f;
    [SerializeField] private float pausaPostEmbestida = 2f;

    [Header("Orientación del Sprite")]
    [SerializeField] private bool miraDerechaPorDefecto = false;

    // Componentes y estados
    private Transform jugador;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D[] colisionesTanque;
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
        spriteRenderer.flipX = !mirandoDerecha;

        if (rb != null)
            rb.freezeRotation = true;
    }

    private void Update()
    {
        if (jugador == null || embistiendo || girando) return;

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
            // 🚶 Prioridad 3: moverse
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

        // 🔹 Limpieza de estados anteriores
        anim.SetBool("Cargar", false);
        anim.SetBool("Volver a disp", false);
        anim.SetBool("Girar", false);

        embistiendo = true;
        atacando = false;
        enCicloAtaque = false;
        puedeEmbestir = false;

        // 🔹 Activa animación de embestida
        anim.SetBool("Embestir", true);
        yield return null; // permite al Animator cambiar de estado
        anim.SetBool("Embestir", false);

        // 🔹 Ignora colisiones mientras embiste
        if (colJugador != null)
            foreach (var cTan in colisionesTanque)
                foreach (var cJug in colJugador)
                    Physics2D.IgnoreCollision(cTan, cJug, true);

        yield return new WaitForSeconds(0.3f);

        // 🔹 Movimiento de embestida
        Vector3 direccion = mirandoDerecha ? Vector3.right : Vector3.left;
        float distanciaRecorrida = 0f;
        Vector3 inicio = transform.position;

        while (distanciaRecorrida < distanciaRecorridoEmbestida)
        {
            transform.Translate(direccion * fuerzaEmbestida * Time.deltaTime, Space.World);
            distanciaRecorrida = Vector3.Distance(inicio, transform.position);
            yield return null;
        }

        // 🔹 Activa animación de giro
        anim.SetBool("Girar", true);

        // 🔹 Restaura colisiones
        if (colJugador != null)
            foreach (var cTan in colisionesTanque)
                foreach (var cJug in colJugador)
                    Physics2D.IgnoreCollision(cTan, cJug, false);

        yield return new WaitForSeconds(pausaPostEmbestida);

        // 🔹 Realiza el cambio de orientación y escala tras el giro
        StartCoroutine(GiroTrasEmbestida());
    }

    private IEnumerator GiroTrasEmbestida()
    {
        yield return new WaitForSeconds(1.0f);

        // Cambia la dirección lógica
        mirandoDerecha = !mirandoDerecha;

        // Fuerza la escala correcta según dirección
        Vector3 escala = transform.localScale;
        escala.x = mirandoDerecha ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;

        yield return new WaitForSeconds(0.3f);

        anim.SetBool("Girar", false);
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
}
