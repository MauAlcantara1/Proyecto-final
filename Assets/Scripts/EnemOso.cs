using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemOso : MonoBehaviour
{
    [Header("Detección del jugador")]
    public Transform jugador;
    public float rangoDeteccion = 5f;
    public float rangoAtaque = 1.5f;

    [Header("Movimiento del oso")]
    public float velocidad = 2f;
    public float velocidadHuida = 5f;
    public bool spriteMiraDerecha = true;

    [Header("Ataque")]
    public int dano = 5;
    public float tiempoPreparacion = 0.8f;
    public float tiempoPostAtaque = 0.8f;
    public float tiempoAgacharse = 0.8f;
    public float tiempoEntreRepeticiones = 0.6f;

    [Header("Vida del oso")]
    public int vidaMax = 100;
    public int vidaActual;
    public int umbralHuida = 30;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool detectando = false;
    private bool preparando = false;
    private bool avanzando = false;
    private bool atacando = false;
    private bool agachandose = false;
    private bool enCicloAtaque = false;
    private bool cayendo = false;
    private bool huyendo = false;

    private Vector3 escalaOriginal;
    private bool ultimoFlip = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        escalaOriginal = transform.localScale;

        if (animator != null)
            animator.applyRootMotion = false;
    }

    void Start()
    {
        vidaActual = vidaMax;

        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                jugador = playerObj.transform;
                Debug.Log("[EnemOso] Jugador asignado automáticamente");
            }
            else
            {
                Debug.LogWarning("[EnemOso] No se encontró jugador con tag 'Player'.");
            }
        }

        animator.Play("Idle");
        Debug.Log("[EnemOso] Iniciado en Idle");
    }

    void Update()
    {
        if (cayendo || huyendo) return; // No hacer nada durante caída o huida

        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Ataque
        if (distancia <= rangoAtaque && !enCicloAtaque && !atacando)
        {
            IniciarCicloAtaque();
            return;
        }

        // Detección y movimiento
        if (distancia <= rangoDeteccion && !detectando && !preparando && !avanzando && !atacando && !agachandose && !enCicloAtaque)
        {
            detectando = true;
            animator.SetTrigger("detEnem");
            Debug.Log("[EnemOso] Jugador detectado → animación detectar enemigo");
            Invoke(nameof(PasarAPrepararCaminar), 1.0f);
        }

        if (distancia > rangoDeteccion && (detectando || preparando || avanzando) && !enCicloAtaque)
        {
            ReiniciarEstadosSinIdle();
        }

        if (avanzando && !atacando && !agachandose && !enCicloAtaque)
        {
            MoverHaciaJugador();
        }
    }

    private void IniciarCicloAtaque()
    {
        enCicloAtaque = true;
        atacando = true;
        detectando = preparando = avanzando = agachandose = false;

        animator.ResetTrigger("detEnem");
        animator.ResetTrigger("Prepcaminar");
        animator.ResetTrigger("Avanzar");
        animator.SetTrigger("PrepAtk");

        Debug.Log("[EnemOso] Jugador en rango → iniciar animación PrepAtk");
        StartCoroutine(CicloDeAtaqueContinuo());
    }

    private IEnumerator CicloDeAtaqueContinuo()
    {
        // 1️⃣ Preparar ataque
        yield return new WaitForSeconds(tiempoPreparacion);

        // 2️⃣ Ataque inicial
        animator.ResetTrigger("PrepAtk");
        animator.SetTrigger("atqEnem");
        Debug.Log("[EnemOso] Ejecutando ataque inicial...");

        if (Vector2.Distance(transform.position, jugador.position) <= rangoAtaque)
        {
            Debug.Log($"[EnemOso] 💥 Daño al jugador: -{dano} HP (ataque inicial)");
        }

        yield return new WaitForSeconds(tiempoPostAtaque);

        // 🔁 3️⃣ Ataque continuo
        while (Vector2.Distance(transform.position, jugador.position) <= rangoAtaque && !cayendo && !huyendo)
        {
            animator.ResetTrigger("atqEnem");
            animator.SetTrigger("RepAtk");
            Debug.Log("[EnemOso] Ataque repetido (RepAtk)");

            yield return new WaitForSeconds(tiempoEntreRepeticiones);

            animator.ResetTrigger("RepAtk");
            animator.SetTrigger("atqEnem");
            Debug.Log("[EnemOso] Ejecutando ataque dentro del bucle...");

            if (Vector2.Distance(transform.position, jugador.position) <= rangoAtaque)
            {
                Debug.Log($"[EnemOso] 💥 Daño al jugador: -{dano} HP (ataque repetido)");
            }

            yield return new WaitForSeconds(tiempoPostAtaque);
        }

        // 4️⃣ Si el jugador se aleja
        atacando = false;
        agachandose = true;
        animator.ResetTrigger("atqEnem");
        animator.SetTrigger("Agacharse");
        Debug.Log("[EnemOso] Jugador se alejó → Agacharse");

        yield return new WaitForSeconds(tiempoAgacharse);

        agachandose = false;
        enCicloAtaque = false;

        if (Vector2.Distance(transform.position, jugador.position) <= rangoDeteccion)
        {
            avanzando = true;
            animator.ResetTrigger("Agacharse");
            animator.SetTrigger("Avanzar");
            Debug.Log("[EnemOso] Regresa a caminar tras agacharse");
        }
        else
        {
            Debug.Log("[EnemOso] Termina agacharse → jugador fuera de rango");
        }
    }

    private void PasarAPrepararCaminar()
    {
        if (!preparando && !avanzando && !atacando && !enCicloAtaque)
        {
            detectando = false;
            preparando = true;
            animator.SetTrigger("Prepcaminar");
            Debug.Log("[EnemOso] Pasando a Prepcaminar");
            Invoke(nameof(PasarAAvanzar), 1.0f);
        }
    }

    private void PasarAAvanzar()
    {
        if (!avanzando && !atacando && !enCicloAtaque)
        {
            avanzando = true;
            preparando = false;
            animator.SetTrigger("Avanzar");
            Debug.Log("[EnemOso] Pasando a Avanzar");
        }
    }

    private void MoverHaciaJugador()
    {
        if (jugador == null) return;

        Vector3 direccion = (jugador.position - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;

        bool flipDeseado = spriteMiraDerecha ? (direccion.x < 0f) : (direccion.x > 0f);
        spriteRenderer.flipX = flipDeseado;
        ultimoFlip = flipDeseado;
    }

    private void ReiniciarEstadosSinIdle()
    {
        detectando = preparando = avanzando = atacando = agachandose = enCicloAtaque = false;

        animator.ResetTrigger("detEnem");
        animator.ResetTrigger("Prepcaminar");
        animator.ResetTrigger("Avanzar");
        animator.ResetTrigger("PrepAtk");
        animator.ResetTrigger("atqEnem");
        animator.ResetTrigger("RepAtk");
        animator.ResetTrigger("Agacharse");

        Debug.Log("[EnemOso] Estados reiniciados (sin forzar Idle).");
    }

    // ==========================================
    // 🔹 SISTEMA DE DAÑO Y HUÍDA
    // ==========================================
    public void RecibirDanio(int cantidad)
    {
        if (vidaActual <= 0 || huyendo) return;

        vidaActual -= cantidad;
        Debug.Log($"[EnemOso] 🩸 Recibe {cantidad} de daño. Vida restante: {vidaActual}");

        if (vidaActual <= umbralHuida && !cayendo && !huyendo)
        {
            StartCoroutine(CaerYHuir());
        }
    }

    private IEnumerator CaerYHuir()
    {
        cayendo = true;
        atacando = false;
        agachandose = false;
        enCicloAtaque = false;
        avanzando = false;

        bool estabaAgachado = animator.GetCurrentAnimatorStateInfo(0).IsName("Agacharse");

        // Limpiar triggers activos
        animator.ResetTrigger("atqEnem");
        animator.ResetTrigger("RepAtk");
        animator.ResetTrigger("PrepAtk");
        animator.ResetTrigger("Avanzar");
        animator.ResetTrigger("Agacharse");

        // Ejecutar caída
        if (estabaAgachado)
            animator.SetTrigger("Caer0");
        else
            animator.SetTrigger("Caer");

        Debug.Log("[EnemOso] 🐻‍❄️ Oso herido → cae");

        yield return new WaitForSeconds(1.0f);

        // Iniciar huida
        cayendo = false;
        huyendo = true;

        if (estabaAgachado)
            animator.SetTrigger("Huir0");
        else
            animator.SetTrigger("Huir");

        Debug.Log("[EnemOso] 🏃‍♂️ Huyendo rápidamente hacia la izquierda");

        while (Vector2.Distance(transform.position, jugador.position) < 10f)
        {
            transform.Translate(Vector2.left * velocidadHuida * Time.deltaTime);
            yield return null;
        }

        Debug.Log("[EnemOso] 🌀 Desaparece tras huir");
        Destroy(gameObject);
    }
}
