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
    [Tooltip("Si la vida baja a este umbral, el oso cae y huye (comportamiento de herido).")]
    public int umbralHuida = 30;

    // Referencias
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisionador;

    // Estados
    private bool invulnerable = true;
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
        colisionador = GetComponent<Collider2D>();
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

        if (animator != null)
            animator.Play("Idle");

        Debug.Log("[EnemOso] Iniciado en Idle");
    }

    void Update()
    {
        if (cayendo || huyendo) return;
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Ataque
        if (distancia <= rangoAtaque && !enCicloAtaque && !atacando)
        {
            IniciarCicloAtaque();
            return;
        }

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
        yield return new WaitForSeconds(tiempoPreparacion);

        animator.ResetTrigger("PrepAtk");
        animator.SetTrigger("atqEnem");
        Debug.Log("[EnemOso] Ejecutando ataque inicial...");

        if (Vector2.Distance(transform.position, jugador.position) <= rangoAtaque)
        {
            Debug.Log($"[EnemOso]  Daño al jugador: -{dano} HP (ataque inicial)");
        }

        yield return new WaitForSeconds(tiempoPostAtaque);

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
                Debug.Log($"[EnemOso]  Daño al jugador: -{dano} HP (ataque repetido)");
            }

            yield return new WaitForSeconds(tiempoPostAtaque);
        }

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
        invulnerable = false; // <--- AHORA YA PUEDE RECIBIR DAÑO

        animator.SetTrigger("Avanzar");
        Debug.Log("[EnemOso] Pasando a Avanzar (invulnerabilidad desactivada)");
    }
}


    private void MoverHaciaJugador()
    {
        if (jugador == null) return;

        Vector3 direccion = (jugador.position - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;

        if (direccion.x < -0.1f)
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        else if (direccion.x > 0.1f)
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
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

    }

public void RecibirDanio(int cantidad)
{
    if (invulnerable) return; 
    if (vidaActual <= 0 || huyendo) return;

    vidaActual -= cantidad;
    if (vidaActual < 0) vidaActual = 0;

    Debug.Log($"[EnemOso]  Recibe {cantidad} de daño. Vida restante: {vidaActual}");

    if (vidaActual <= umbralHuida && !cayendo && !huyendo)
    {
        StartCoroutine(CaerYHuir());
    }
}


        private IEnumerator CaerYHuir()
    {
        Debug.Log("[EnemOso] → CaerYHuir() iniciado");

        cayendo = true;
        atacando = false;
        agachandose = false;
        enCicloAtaque = false;
        avanzando = false;

        bool estabaAgachado = animator.GetCurrentAnimatorStateInfo(0).IsName("Agacharse");

        animator.ResetTrigger("atqEnem");
        animator.ResetTrigger("RepAtk");
        animator.ResetTrigger("PrepAtk");
        animator.ResetTrigger("Avanzar");
        animator.ResetTrigger("Agacharse");

        if (estabaAgachado)
            animator.SetTrigger("Caer0");
        else
            animator.SetTrigger("Caer");

        Debug.Log("[EnemOso]  Oso herido → cae");

        yield return new WaitForSeconds(1.0f);

        cayendo = false;
        huyendo = true;

        DropLoot drop = GetComponent<DropLoot>() ?? GetComponentInChildren<DropLoot>();
        if (drop != null)
        {
            drop.SoltarObjetos();
        }


        if (estabaAgachado)
            animator.SetTrigger("Huir0");
        else
            animator.SetTrigger("Huir");

        Vector2 direccionHuida = (transform.position.x < jugador.position.x) ? Vector2.left : Vector2.right;

        if (direccionHuida == Vector2.left)
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);

        Debug.Log($"[EnemOso]  Huyendo hacia {(direccionHuida == Vector2.left ? "izquierda" : "derecha")}");

        while (jugador != null && Vector2.Distance(transform.position, jugador.position) < 14f)
        {
            transform.Translate(direccionHuida * velocidadHuida * Time.deltaTime);
            yield return null;
        }



        Destroy(gameObject);
    }

}
