using UnityEngine;

public class Tanque : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float distanciaDeteccion = 15f;
    [SerializeField] private float distanciaFrenado = 10f;

    [Header("Vida y Daño")]
    [SerializeField] private int vidaMaxima = 10;
    private int vidaActual;
    private bool estaMuerto = false;

    [Header("Disparo")]
    [SerializeField] private GameObject prefabBala;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float fuerzaDisparo = 8f;

    [Header("Detección de giro")]
    [Tooltip("Ángulo mínimo para considerar que el jugador está detrás (en grados)")]
    [SerializeField] private float anguloDetras = 100f;

    [Header("Audio")]
    [SerializeField] private AudioClip sonidoMuerte;
    private AudioSource audioSource;

    private Animator animator;
    private Transform jugador;
    private Transform jugador1;
    private Transform jugador2;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisionador;
    private Rigidbody2D rb;

    private bool persiguiendo = false;
    private bool arranqueOriginalHecho = false;
    private bool arranqueOriginalTerminado = false;

    private bool frenando = false;
    private bool prepHecho = false;
    private bool disparoHecho = false;

    private bool arranque0Activo = false;
    private bool arranque0Terminado = false;
    private bool enProcesoAtaque = false;
    private bool girando = false;

    private bool mirandoDerecha = true;

    private Vector3 puntoInicial;
    private Vector3 puntoFinal;
    private bool yendoAlFinal = true;

    private bool balaDisparada = false;

    private void Start()
    {
        GameObject j1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject j2 = GameObject.FindGameObjectWithTag("Player2");

        if (j1 != null) jugador1 = j1.transform;
        if (j2 != null) jugador2 = j2.transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colisionador = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        // 🔊 AUDIO
        audioSource = GetComponent<AudioSource>();

        puntoInicial = transform.position;
        puntoFinal = puntoInicial + transform.right * 4f;

        vidaActual = vidaMaxima;
        Debug.Log("✅ Tanque iniciado con vida: " + vidaActual);
    }

    private void Update()
    {
        jugador = ObtenerJugadorObjetivo();
        if (estaMuerto) return;

        DetectarJugador();
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if (arranqueOriginalHecho && !arranqueOriginalTerminado)
        {
            if (info.IsName("Arranque") && info.normalizedTime >= 0.95f)
            {
                arranqueOriginalTerminado = true;
                animator.SetTrigger("Mover");
            }
        }

        if (arranque0Activo && !arranque0Terminado)
        {
            if (info.IsName("Arranque0") && info.normalizedTime >= 0.95f)
            {
                arranque0Terminado = true;
                animator.SetTrigger("Mover1");
            }
        }

        if (frenando && !prepHecho && !girando)
        {
            if (info.IsName("Frenado") && info.normalizedTime >= 0.95f)
            {
                enProcesoAtaque = true;
                animator.ResetTrigger("Frenado");

                if (JugadorDetras())
                {
                    girando = true;
                    animator.SetTrigger("Girar");
                }
                else
                {
                    prepHecho = true;
                    animator.SetTrigger("Preparacion");
                }
            }
        }

        if (girando && info.IsName("Giro"))
        {
            if (info.normalizedTime >= 0.95f)
            {
                FlipSprite();
                girando = false;
                prepHecho = true;
                animator.ResetTrigger("Girar");
                animator.SetTrigger("Preparacion");
            }
        }

        if (prepHecho && !disparoHecho)
        {
            if (info.IsName("Prep") && info.normalizedTime >= 0.95f)
            {
                disparoHecho = true;
                enProcesoAtaque = true;
                animator.ResetTrigger("Preparacion");
                animator.SetTrigger("Disparo");
            }
        }

        if (info.IsName("Disparo") && info.normalizedTime >= 0.95f)
        {
            balaDisparada = false;
        }

        if (disparoHecho)
        {
            if (info.IsName("Disparo") && info.normalizedTime >= 0.95f)
            {
                balaDisparada = false;

                arranque0Activo = true;
                arranque0Terminado = false;

                frenando = false;
                prepHecho = false;
                disparoHecho = false;
                enProcesoAtaque = false;
                girando = false;

                animator.ResetTrigger("Disparo");
                animator.SetTrigger("Arranque1");
            }
        }

        if (!enProcesoAtaque && persiguiendo && (arranqueOriginalTerminado || arranque0Terminado))
        {
            float distancia = Vector3.Distance(transform.position, jugador.position);

            if (distancia <= distanciaFrenado)
            {
                frenando = true;
                enProcesoAtaque = true;
                animator.SetTrigger("Frenado");
                return;
            }

            if (!info.IsName("Arranque") &&
                !info.IsName("Arranque0") &&
                !info.IsName("Frenado") &&
                !info.IsName("Prep") &&
                !info.IsName("Disparo") &&
                !info.IsName("Giro"))
            {
                PerseguirJugador();
            }
        }
        else if (!persiguiendo && !enProcesoAtaque)
        {
            Patrullar();
        }
    }

    private void Disparar()
    {
        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, puntoDisparo.rotation);

        Rigidbody2D rbBala = bala.GetComponent<Rigidbody2D>();
        if (rbBala != null)
        {
            Vector2 direccion = mirandoDerecha ? Vector2.left : Vector2.right;
            rbBala.linearVelocity = direccion * fuerzaDisparo;
        }
    }

    public void RecibirDaño(int cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;

        if (vidaActual <= 0) Morir();
    }

    private void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        Debug.Log("Tanque ha muerto.");

        if (audioSource != null && sonidoMuerte != null)
            audioSource.PlayOneShot(sonidoMuerte);

        // Detener movimiento y evitar físicas
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; 
        }

        // Evitar colisiones
        if (colisionador != null)
        {
            colisionador.enabled = false;
        }

        animator.SetTrigger("Muerte");

        StopAllCoroutines();

        DropLoot drop = GetComponent<DropLoot>();
        if (drop != null)
            drop.SoltarObjetos();
    }


    private void DetectarJugador()
    {
        if (jugador == null) return;

        float dist = Vector3.Distance(transform.position, jugador.position);

        if (dist <= distanciaDeteccion)
        {
            persiguiendo = true;

            if (!arranqueOriginalHecho)
            {
                arranqueOriginalHecho = true;
                arranqueOriginalTerminado = false;
                animator.SetTrigger("Arranque");
            }
        }
        else
        {
            persiguiendo = false;

            if (!enProcesoAtaque)
            {
                arranque0Activo = false;
                arranque0Terminado = false;
                frenando = false;
                prepHecho = false;
                disparoHecho = false;
                girando = false;

                animator.ResetTrigger("Arranque");
                animator.ResetTrigger("Arranque1");
                animator.ResetTrigger("Frenado");
                animator.ResetTrigger("Preparacion");
                animator.ResetTrigger("Disparo");
                animator.ResetTrigger("Girar");
            }
        }
    }

    private bool JugadorDetras()
    {
        if (jugador == null) return false;

        Vector2 dirToPlayer = (jugador.position - transform.position).normalized;
        Vector2 facing = mirandoDerecha ? Vector2.right : Vector2.left;
        float angle = Vector2.Angle(facing, dirToPlayer);
        return angle < anguloDetras;
    }

    private void FlipSprite()
    {
        VoltearTanque(!mirandoDerecha);
    }

    private void PerseguirJugador()
    {
        if (jugador == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            jugador.position,
            velocidad * Time.deltaTime
        );
    }

    private void Patrullar()
    {
        if (arranqueOriginalHecho) return;

        Vector3 objetivo = yendoAlFinal ? puntoFinal : puntoInicial;
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);

        if (Vector3.Distance(transform.position, objetivo) < 0.1f)
            yendoAlFinal = !yendoAlFinal;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaFrenado);
    }

    private void VoltearTanque(bool mirarDerecha)
    {
        mirandoDerecha = mirarDerecha;

        Vector3 escala = transform.localScale;
        escala.x = mirandoDerecha ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;

        Vector3 pos = puntoDisparo.localPosition;
        pos.x = -pos.x;
        puntoDisparo.localPosition = pos;
    }

    private Transform ObtenerJugadorObjetivo()
    {
        if (jugador1 == null && jugador2 == null)
            return null;

        if (jugador1 != null && jugador2 == null)
            return jugador1;

        if (jugador2 != null && jugador1 == null)
            return jugador2;

        float dist1 = Vector2.Distance(transform.position, jugador1.position);
        float dist2 = Vector2.Distance(transform.position, jugador2.position);

        return dist1 < dist2 ? jugador1 : jugador2;
    }

}
