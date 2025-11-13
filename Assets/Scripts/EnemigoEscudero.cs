using UnityEngine;

public class EnemigoEscudero : MonoBehaviour
{
    public float velocidad = 2f;
    public float rangoDeteccion = 5f;
    public float rangoAtaque = 1.5f;
    public Rigidbody2D rb;
    public Animator animator;

    private Transform jugador;
    private bool jugadorDetectado = false;
    private bool enRangoAtaque = false;

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
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

        if (jugadorDetectado && !enRangoAtaque)
        {
            // Caminar hacia el jugador
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
        direccion.y = 0f; // ❌ Ignorar diferencia en Y
        direccion.Normalize();

        transform.position += direccion * velocidad * Time.deltaTime;
    }

    private void FlipTowardsPlayer()
    {
        if (jugador == null)
            return;

        if (jugador.position.x < transform.position.x)
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        else
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
    }
}
