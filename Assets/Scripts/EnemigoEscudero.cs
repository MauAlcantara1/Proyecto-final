using UnityEngine;

public class EnemigoEscudero : MonoBehaviour
{
    [Header("Movimiento y detección")]
    public float velocidad = 2f;
    public float rangoDeteccion = 5f;
    public float rangoAtaque = 1.5f;

    [Header("Daño")]
    public int dañoAtaque = 1;

    [Header("Referencias")]
    public Rigidbody2D rb;
    public Animator animator;
    public Collider2D hitboxMachete;
    
    private Transform jugador;
    private bool estaAtacando = false;
    private bool jugadorDetectado = false;
    private bool enRangoAtaque = false;

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        hitboxMachete.enabled = false;
    }

    private void Update()
    {
        if (jugador == null)
            return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        jugadorDetectado = distancia <= rangoDeteccion;
        enRangoAtaque = distancia <= rangoAtaque;

        animator.SetBool("jugadorDetectado", jugadorDetectado);
        animator.SetBool("enRangoAtaque", enRangoAtaque);

        if (estaAtacando)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (jugadorDetectado && !enRangoAtaque)
        {
            animator.SetBool("estaCaminando", true);
            MoverHaciaJugador();
        }
        else
        {
            animator.SetBool("estaCaminando", false);
            rb.linearVelocity = Vector2.zero;
        }

        FlipTowardsPlayer();
    }

    private void MoverHaciaJugador()
    {
        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0f;
        direccion.Normalize();

        transform.position += direccion * velocidad * Time.deltaTime;
    }

    private void FlipTowardsPlayer()
    {
        if (jugador.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    public void IniciarAtaque()
    {
        estaAtacando = true;
    }

    public void FinalizarAtaque()
    {
        estaAtacando = false;
    }

    public void ActivarHitbox()
    {
        hitboxMachete.enabled = true;
    }

    public void DesactivarHitbox()
    {
        hitboxMachete.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitboxMachete.enabled && other.CompareTag("Player"))
        {
            VidasPlayer vidaPlayer = other.GetComponent<VidasPlayer>();
            if (vidaPlayer != null)
            {
                vidaPlayer.TomarDaño(dañoAtaque);
            }
        }
    }
}
