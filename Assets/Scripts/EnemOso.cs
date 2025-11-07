using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemOso : MonoBehaviour
{
    [Header("Detección del jugador (Horizontal)")]
    public Transform jugador;
    public float rangoDeteccion = 6f;
    public float rangoAtaque = 4f;

    [Header("Detección por área (Nuevo)")]
    public LayerMask capaJugador;
    public float radioDeteccionCircular = 5f;

    [Header("Movimiento del oso")]
    public float velocidad = 2f;
    public float velocidadHuida = 5f;
    public bool spriteMiraDerecha = true;

    [Header("Tiempos de animaciones")]
    public float tiempoPreparacion = 0.8f;
    public float tiempoPostAtaque = 0.8f;
    public float tiempoAgacharse = 0.8f;
    public float tiempoEntreRepeticiones = 0.6f;
    public float tiempoGiro = 0.9f;
    public float tiempoCarga = 1.0f;

    [Header("Ataque")]
    public int dano = 5;

    [Header("Vida del oso")]
    public int vidaMax = 100;
    public int vidaActual;
    public int umbralHuida = 30;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisionador;

    private bool detectando = false;
    private bool preparando = false;
    private bool avanzando = false;
    private bool atacando = false;
    private bool agachandose = false;
    private bool enCicloAtaque = false;
    private bool cayendo = false;
    private bool huyendo = false;
    private bool haciendoGiro = false;
    private bool haciendoCarga = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colisionador = GetComponent<Collider2D>();
    }

    private void Start()
    {
        vidaActual = vidaMax;

        if (jugador == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                jugador = p.transform;
                Debug.Log("[OSO] Jugador asignado automáticamente.");
            }
        }

        animator.Play("Idle");
    }

    private void Update()
    {
        if (cayendo || huyendo || haciendoGiro || haciendoCarga) return;
        if (jugador == null) return;

        float distancia = Mathf.Abs(jugador.position.x - transform.position.x);

        // NUEVO: Detección circular
        bool jugadorCerca = Physics2D.OverlapCircle(transform.position, radioDeteccionCircular, capaJugador);

        // Activar ciclo de ataque como antes
        if (jugadorCerca && distancia <= rangoAtaque && !enCicloAtaque && !atacando)
        {
            Debug.Log("[OSO] Jugador en rango -> iniciar ciclo de ataque");
            IniciarCicloAtaque();
            return;
        }

        // Detección inicial usando detección circular
        if (jugadorCerca && distancia <= rangoDeteccion && !detectando && !preparando && !avanzando && !enCicloAtaque && !atacando)
        {
            detectando = true;
            animator.SetTrigger("detEnem");
            Debug.Log("[OSO] Detecta jugador → detEnem");
            Invoke(nameof(PasarAPrepararCaminar), 1.0f);
        }

        // Perder detección cuando sale del radio
        if ((!jugadorCerca || distancia > rangoDeteccion) && (detectando || preparando || avanzando) && !enCicloAtaque)
        {
            ReiniciarEstadosSinIdle();
        }

        if (avanzando && !atacando && !enCicloAtaque)
        {
            MoverHaciaJugador();
        }
    }

    private void IniciarCicloAtaque()
    {
        enCicloAtaque = true;
        atacando = true;

        detectando = preparando = avanzando = false;

        animator.SetTrigger("PrepAtk");
        Debug.Log("[OSO] -> PrepAtk");

        StartCoroutine(CicloAtaque());
    }

    private IEnumerator CicloAtaque()
    {
        yield return new WaitForSeconds(tiempoPreparacion);

        animator.ResetTrigger("PrepAtk");
        animator.SetTrigger("atqEnem");
        Debug.Log("[OSO] -> atqEnem (Ataque inicial)");

        yield return new WaitForSeconds(tiempoPostAtaque);

        while (Mathf.Abs(jugador.position.x - transform.position.x) <= rangoAtaque)
        {
            animator.ResetTrigger("atqEnem");
            animator.SetTrigger("RepAtk");
            Debug.Log("[OSO] -> RepAtk");

            yield return new WaitForSeconds(tiempoEntreRepeticiones);

            animator.ResetTrigger("RepAtk");
            animator.SetTrigger("atqEnem");
            Debug.Log("[OSO] -> atqEnem (repetición)");

            yield return new WaitForSeconds(tiempoPostAtaque);
        }

        atacando = false;
        agachandose = true;

        animator.SetTrigger("Agacharse");
        Debug.Log("[OSO] -> Agacharse");

        yield return new WaitForSeconds(tiempoAgacharse);

        agachandose = false;
        enCicloAtaque = false;

        IniciarGiro();
    }

    private void IniciarGiro()
    {
        haciendoGiro = true;
        animator.SetTrigger("Girar");
        Debug.Log("[OSO] -> Giro");

        StartCoroutine(GiroRoutine());
    }

    private IEnumerator GiroRoutine()
    {
        yield return new WaitForSeconds(tiempoGiro);

        spriteRenderer.flipX = !spriteRenderer.flipX;
        Debug.Log("[OSO] Giro completado. Flip aplicado.");

        haciendoGiro = false;
        IniciarCarga();
    }

    private void IniciarCarga()
    {
        haciendoCarga = true;
        animator.SetTrigger("Cargar");
        Debug.Log("[OSO] -> Carga");

        StartCoroutine(CargaRoutine());
    }

    private IEnumerator CargaRoutine()
    {
        yield return new WaitForSeconds(tiempoCarga);
        haciendoCarga = false;
    }

    private void MoverHaciaJugador()
    {
        Vector3 direccion = new Vector3(jugador.position.x - transform.position.x, 0, 0).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;

        bool flipDeseado = spriteMiraDerecha ? (direccion.x < 0f) : (direccion.x > 0f);
        spriteRenderer.flipX = flipDeseado;
    }

    private void PasarAPrepararCaminar()
    {
        if (!preparando && !avanzando && !enCicloAtaque)
        {
            detectando = false;
            preparando = true;
            animator.SetTrigger("Prepcaminar");
            Debug.Log("[OSO] -> Prepcaminar");
            Invoke(nameof(PasarAAvanzar), 1.0f);
        }
    }

    private void PasarAAvanzar()
    {
        if (!avanzando && !atacando && !enCicloAtaque)
        {
            preparando = false;
            avanzando = true;
            animator.SetTrigger("Avanzar");
            Debug.Log("[OSO] -> Avanzar");
        }
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
        Debug.Log("[OSO] Estados reiniciados.");
    }

    public void RecibirDanio(int cantidad)
    {
        if (vidaActual <= 0 || huyendo) return;

        vidaActual -= cantidad;
        Debug.Log("[OSO] Recibe daño: " + cantidad + " Vida: " + vidaActual);

        if (vidaActual <= umbralHuida && !huyendo)
        {
            StartCoroutine(CaerYHuir());
        }
    }

    private IEnumerator CaerYHuir()
    {
        cayendo = true;
        atacando = agachandose = enCicloAtaque = avanzando = false;

        animator.SetTrigger("Caer");
        Debug.Log("[OSO] -> Caer");

        yield return new WaitForSeconds(1f);

        colisionador.enabled = false;
        Debug.Log("[OSO] Collider desactivado");

        cayendo = false;
        huyendo = true;

        animator.SetTrigger("Huir");
        Debug.Log("[OSO] -> Huir");

        while (Mathf.Abs(jugador.position.x - transform.position.x) < 10f)
        {
            transform.Translate(Vector2.left * velocidadHuida * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioDeteccionCircular);
    }
}
