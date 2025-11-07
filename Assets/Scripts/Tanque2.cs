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

    [Header("Tiempos (Inspector)")]
    [SerializeField] private float duracionCarga = 1.5f;
    [SerializeField] private float duracionDisparo = 1.5f; // fallback, se intenta leer de Animator
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

    // Estados exclusivos
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

        // Protección extra: si el animator está actualmente en Disparo o Cargar, bloqueamos movimiento
        if (IsInState("DispEnem") || IsInState("Cargar"))
        {
            DetenerMovimiento();
            return;
        }

        // Si cualquiera de las banderas bloqueantes está activo, no mover
        if (cargando || embistiendo || disparando || enDecision || girando)
        {
            DetenerMovimiento();
            return;
        }

        // Movimiento hacia el jugador
        if (distancia < distanciaDeteccion && distancia > distanciaParada)
        {
            TrySetTriggerSafe("DetEnem");
            MoverHaciaJugador(velocidadMovimiento);
        }
        else if (distancia <= distanciaParada)
        {
            DetenerMovimiento();

            if (!enDecision)
            {
                TrySetTriggerSafe("DecidirAtk");
                StartCoroutine(DecidirAtaque());
            }
        }
        else
        {
            DetenerMovimiento();
        }
    }

    // --------------------------------------------------------------------------------
    // Helpers
    // --------------------------------------------------------------------------------
    private bool IsInState(string stateName)
    {
        if (anim == null) return false;
        var info = anim.GetCurrentAnimatorStateInfo(0);
        return info.IsName(stateName);
    }

    private void TrySetTriggerSafe(string trigger)
    {
        anim.ResetTrigger(trigger);
        anim.SetTrigger(trigger);
    }

    // --------------------------------------------------------------------------------
    // Movimiento / Flip
    // --------------------------------------------------------------------------------
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

    // --------------------------------------------------------------------------------
    // Decisión y ciclos de ataque
    // --------------------------------------------------------------------------------
    private IEnumerator DecidirAtaque()
    {
        enDecision = true;
        yield return new WaitForSeconds(0.5f);

        int eleccion = Random.Range(0, 2); // 0 = Cargar+Disparo, 1 = Embestir
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
        anim.ResetTrigger("Embestir");
        TrySetTriggerSafe("Cargar");
        Debug.Log("[Tanque2] Cargar trigger enviado");

        yield return new WaitUntil(() => IsInState("Cargar"));
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        cargando = false;

        disparando = true;
        TrySetTriggerSafe("DispEnem");
        Debug.Log("[Tanque2] Disparo trigger enviado");

        yield return new WaitUntil(() => IsInState("DispEnem"));

        float start = Time.time;
        float clipLen = anim.GetCurrentAnimatorStateInfo(0).length;
        if (clipLen <= 0f) clipLen = duracionDisparo;

        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f ||
            Time.time - start > clipLen + 0.2f);

        disparando = false;

        int siguiente = Random.Range(0, 2);
        if (siguiente == 0)
        {
            Debug.Log("[Tanque2] Volver a Cargar after Disparo");
            TrySetTriggerSafe("Volver a disp");
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(AtaqueCargarYDisparar());
        }
        else
        {
            Debug.Log("[Tanque2] Decide Embestir después de Disparo");
            yield return StartCoroutine(Embestida());
        }
    }

    // --------------------------------------------------------------------------------
    // Embestida
    // --------------------------------------------------------------------------------
    private IEnumerator Embestida()
    {
        if (embistiendo || cargando || disparando || girando)
        {
            Debug.Log("[Tanque2] Embestida cancelada: estado incompatible");
            yield break;
        }

        embistiendo = true;
        puedeHacerDaño = true;
        dañoAplicado = false;

        TrySetTriggerSafe("Embestir");
        Debug.Log("[Tanque2] Embestir trigger enviado");

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

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Debug.Log("[Tanque2] Embestida finalizada. Iniciando Giro...");
        yield return StartCoroutine(Giro());
    }

    // --------------------------------------------------------------------------------
    // Giro
    // --------------------------------------------------------------------------------
    private IEnumerator Giro()
    {
        girando = true;
        anim.ResetTrigger("Embestir");
        anim.ResetTrigger("Cargar");
        TrySetTriggerSafe("Girar");

        Debug.Log("[Tanque2] Giro trigger enviado, esperando entrada a estado 'Giro'...");
        yield return new WaitUntil(() => IsInState("Giro"));
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        anim.ResetTrigger("Girar");
        spriteRenderer.flipX = !spriteRenderer.flipX;
        Debug.Log("[Tanque2] Giro completado. flipX invertido.");

        girando = false;
        yield return StartCoroutine(ForzarIrACargar());
    }

    private IEnumerator ForzarIrACargar()
    {
        if (cargando || disparando || embistiendo || girando)
            yield break;

        cargando = true;
        TrySetTriggerSafe("Cargar");
        Debug.Log("[Tanque2] Forzando Cargar después de Giro...");

        yield return new WaitUntil(() => IsInState("Cargar"));
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        cargando = false;
        enDecision = false;
        yield return null;
    }

    // --------------------------------------------------------------------------------
    // Colisiones (daño)
    // --------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (puedeHacerDaño && !dañoAplicado && other.CompareTag("Player"))
        {
            Debug.Log($"[Tanque2] 💥 Golpea al jugador y causa {danoEmbestida} de daño");
            dañoAplicado = true;
        }
        else if (other.CompareTag("Player") && !puedeHacerDaño)
        {
            Debug.Log("[Tanque2] Colisión con Player detectada (sin daño)");
        }
    }
}
