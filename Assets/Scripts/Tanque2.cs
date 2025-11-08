using System.Collections;
using UnityEngine;

public class Tanque2 : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 2f;
    [SerializeField] private float velocidadEmbestida = 8f;
    [SerializeField] private float distanciaDeteccion = 30f;
    [SerializeField] private float distanciaParada = 20f;
    [SerializeField] private float distanciaEmbestida = 20f;

    [Header("Daño Embestida")]
    [SerializeField] private float danoEmbestida = 5f;

    [Header("Tiempos (ajustables en Inspector)")]
    [SerializeField] private float duracionCarga = 1.5f;
    [SerializeField] private float duracionDisparo = 1.5f;
    [SerializeField] private float retrasoAntesEmbestir = 0.3f;
    [SerializeField] private float duracionGiroFallback = 1.5f;
    [SerializeField] private float tiempoEntreDecisiones = 1f;

    [Header("Sprite / Flip")]
    [SerializeField] private bool spriteMiraDerecha = true;

    private Transform jugador;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D[] colisionesTanque;

    private bool embistiendo = false;
    private bool cargando = false;
    private bool disparando = false;
    private bool enDecision = false;
    private bool girando = false;
    private bool puedeHacerDaño = false;
    private bool dañoAplicado = false;

    private Vector2 direccionActual;

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colisionesTanque = GetComponentsInChildren<Collider2D>();

        if (anim != null)
            anim.applyRootMotion = false;
    }

    private void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (cargando || embistiendo || disparando || enDecision || girando)
        {
            DetenerMovimiento();
            return;
        }

        if (distancia < distanciaDeteccion && distancia > distanciaParada)
        {
            anim.SetTrigger("DetEnem");
            MoverHaciaJugador(velocidadMovimiento);
        }
        else if (distancia <= distanciaParada)
        {
            DetenerMovimiento();
            if (!enDecision)
            {
                anim.SetTrigger("DecidirAtk");
                StartCoroutine(DecidirAtaque());
            }
        }
        else
        {
            DetenerMovimiento();
        }
    }

    private void MoverHaciaJugador(float velocidad)
    {
        Vector3 direccion = (jugador.position - transform.position).normalized;
        direccionActual = direccion;

        bool flipDeseado = spriteMiraDerecha ? (direccion.x < 0f) : (direccion.x > 0f);
        spriteRenderer.flipX = flipDeseado;

        transform.Translate(direccion * velocidad * Time.deltaTime);
    }

    private void DetenerMovimiento()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator DecidirAtaque()
    {
        enDecision = true;
        yield return new WaitForSeconds(0.5f);

        int eleccion = Random.Range(0, 2); // 0 carga, 1 embestida
        Debug.Log($"[Tanque2] Elección: {(eleccion == 0 ? "Cargar+Disparar" : "Embestir")}");

        if (eleccion == 0)
        {
            yield return StartCoroutine(AtaqueCargarYDisparar());
        }
        else
        {
            if (!cargando && !disparando)
                yield return StartCoroutine(Embestida());
        }

        yield return new WaitForSeconds(tiempoEntreDecisiones);
        enDecision = false;
    }

    private IEnumerator AtaqueCargarYDisparar()
    {
        cargando = true;
        anim.SetTrigger("Cargar");
        Debug.Log("[Tanque2] Cargando...");
        yield return new WaitForSeconds(duracionCarga);
        cargando = false;

        disparando = true;
        anim.SetTrigger("DispEnem");
        Debug.Log("[Tanque2] Disparando...");
        yield return new WaitForSeconds(duracionDisparo);
        disparando = false;

        int siguiente = Random.Range(0, 2);
        if (siguiente == 0)
        {
            Debug.Log("[Tanque2] Volver a Cargar");
            anim.SetTrigger("Volver a disp");
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(AtaqueCargarYDisparar());
        }
        else
        {
            Debug.Log("[Tanque2] Decide Embestir después de Disparo");
            yield return StartCoroutine(Embestida());
        }
    }

    private IEnumerator Embestida()
    {
        if (embistiendo || cargando || disparando || girando)
            yield break;

        embistiendo = true;
        puedeHacerDaño = true;
        dañoAplicado = false;

        anim.SetTrigger("Embestir");
        Debug.Log("[Tanque2] Embestida iniciada");

        Collider2D[] colJugador = jugador.GetComponentsInChildren<Collider2D>();
        foreach (var cTan in colisionesTanque)
            foreach (var cJug in colJugador)
                Physics2D.IgnoreCollision(cTan, cJug, true);

        yield return new WaitForSeconds(retrasoAntesEmbestir);

        Vector3 direccion = (jugador.position - transform.position).normalized;

        bool flipDeseado = spriteMiraDerecha ? (direccion.x < 0f) : (direccion.x > 0f);
        spriteRenderer.flipX = flipDeseado;

        float distanciaRecorrida = 0f;
        Vector3 inicio = transform.position;

        while (distanciaRecorrida < distanciaEmbestida)
        {
            transform.Translate(direccion * velocidadEmbestida * Time.deltaTime);
            distanciaRecorrida = Vector3.Distance(inicio, transform.position);
            yield return null;
        }

        embistiendo = false;
        puedeHacerDaño = false;

        foreach (var cTan in colisionesTanque)
            foreach (var cJug in colJugador)
                Physics2D.IgnoreCollision(cTan, cJug, false);

        if (rb != null) rb.linearVelocity = Vector2.zero;

        Debug.Log("[Tanque2] Embestida finalizada. Iniciando giro...");

        yield return StartCoroutine(Giro());
    }

    private IEnumerator Giro()
    {
        girando = true;
        anim.SetTrigger("Girar");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Giro"));

        float durGiro = anim.GetCurrentAnimatorStateInfo(0).length;
        if (durGiro <= 0f) durGiro = duracionGiroFallback;

        yield return new WaitForSeconds(durGiro);

        spriteRenderer.flipX = !spriteRenderer.flipX;

        Debug.Log("[Tanque2] Giro completado. Ahora → Cargar");

        girando = false;

        // ✅ VERSION C → Después de girar, SIEMPRE pasa a Cargar
        yield return StartCoroutine(ForzarIrACargar());
    }

    private IEnumerator ForzarIrACargar()
    {
        cargando = true;
        anim.SetTrigger("Cargar");

        Debug.Log("[Tanque2] Entrando a Carga después del giro");

        yield return new WaitForSeconds(duracionCarga);

        cargando = false;

        // ✅ Después vuelve al ciclo normal (DecidirAtaque)
        enDecision = false;
        yield return StartCoroutine(DecidirAtaque());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (puedeHacerDaño && !dañoAplicado && other.CompareTag("Player"))
        {
            Debug.Log($"[Tanque2] 💥 Golpea al jugador y causa {danoEmbestida} de daño");
            dañoAplicado = true;
        }
    }
}
