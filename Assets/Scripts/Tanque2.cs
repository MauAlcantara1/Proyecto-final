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
    [SerializeField] private float distanciaEmbestida = 10f;
    [SerializeField] private float fuerzaEmbestida = 8f;
    [SerializeField] private float distanciaRecorridoEmbestida = 20f;
    [SerializeField] private float pausaPostEmbestida = 2f;

    [Header("Orientación del Sprite")]
    [SerializeField] private bool miraDerechaPorDefecto = false;

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
        if (jugador == null || embistiendo || girando)
            return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Detección inicial
        if (!jugadorDetectado && distancia <= distanciaDeteccion)
        {
            jugadorDetectado = true;
            anim.SetTrigger("DetEnem");
            enMovimiento = true;
        }

        // Movimiento hacia el jugador
        if (enMovimiento && !atacando)
        {
            if (distancia > distanciaParada)
                MoverHaciaJugador();
            else
            {
                DetenerMovimiento();
                enMovimiento = false;
                anim.SetTrigger("DecidirAtk");

                if (!enCicloAtaque)
                    Invoke(nameof(IniciarAtaque), retrasoAntesDeAtacar);
            }
        }

        // Si jugador se aleja mucho → reset total
        if (jugadorDetectado && distancia > distanciaDeteccion + 5f)
        {
            jugadorDetectado = false;
            CancelInvoke(nameof(IniciarAtaque));
            ResetEstados();
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

    private void IniciarAtaque()
    {
        if (enCicloAtaque || embistiendo || girando) return;
        enCicloAtaque = true;
        atacando = true;
        StartCoroutine(CicloAtaque());
    }

    private IEnumerator CicloAtaque()
    {
        while (jugadorDetectado && !embistiendo && enCicloAtaque)
        {
            float distancia = Vector2.Distance(transform.position, jugador.position);

            // Si el jugador está muy cerca → embestir
            if (puedeEmbestir && distancia <= distanciaEmbestida)
            {
                enCicloAtaque = false;
                atacando = false;
                StartCoroutine(Embestida());
                yield break;
            }

            // Fase de carga
            anim.SetTrigger("Cargar");
            yield return new WaitForSeconds(duracionCarga);

            if (!enCicloAtaque || embistiendo) yield break;

            // Fase de disparo
            anim.SetTrigger("DispEnem");
            float tiempo = 0f;

            while (tiempo < duracionDisparo)
            {
                if (jugador != null && puedeEmbestir && Vector2.Distance(transform.position, jugador.position) <= distanciaEmbestida)
                {
                    enCicloAtaque = false;
                    atacando = false;
                    StartCoroutine(Embestida());
                    yield break;
                }

                tiempo += Time.deltaTime;
                yield return null;
            }

            // Vuelve a carga (no embestida)
            anim.SetTrigger("Volver a disp");
            yield return new WaitForSeconds(pausaEntreCiclos);
        }

        ResetEstados();
    }

    private IEnumerator Embestida()
    {
        if (embistiendo) yield break;

        embistiendo = true;
        atacando = true;
        enCicloAtaque = false;
        puedeEmbestir = false;

        anim.SetTrigger("Embestir");

        // Ignorar colisión con el jugador durante embestida
        if (colJugador != null)
            foreach (var cTan in colisionesTanque)
                foreach (var cJug in colJugador)
                    Physics2D.IgnoreCollision(cTan, cJug, true);

        yield return new WaitForSeconds(0.5f);

        Vector3 direccion = mirandoDerecha ? Vector3.right : Vector3.left;
        float distanciaRecorrida = 0f;
        Vector3 inicio = transform.position;

        while (distanciaRecorrida < distanciaRecorridoEmbestida)
        {
            transform.Translate(direccion * fuerzaEmbestida * Time.deltaTime, Space.World);
            distanciaRecorrida = Vector3.Distance(inicio, transform.position);
            yield return null;
        }

        // Reactivar colisiones
        if (colJugador != null)
            foreach (var cTan in colisionesTanque)
                foreach (var cJug in colJugador)
                    Physics2D.IgnoreCollision(cTan, cJug, false);

        yield return new WaitForSeconds(pausaPostEmbestida);
        StartCoroutine(GiroTrasEmbestida());
    }

    private IEnumerator GiroTrasEmbestida()
    {
        girando = true;
        anim.SetTrigger("Girar");
        yield return new WaitForSeconds(1.2f);

        mirandoDerecha = !mirandoDerecha;
        spriteRenderer.flipX = !spriteRenderer.flipX;

        yield return new WaitForSeconds(0.4f);

        girando = false;
        embistiendo = false;
        atacando = false;

        anim.SetTrigger("Volver a disp");
        puedeEmbestir = true;
        enCicloAtaque = false;

        // 🔁 Reinicia ciclo de disparo
        Invoke(nameof(IniciarAtaque), 1.5f);
    }

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
