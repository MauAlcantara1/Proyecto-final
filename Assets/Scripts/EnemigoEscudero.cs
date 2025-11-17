using UnityEngine;

public class EnemigoEscudero : MonoBehaviour
{
    public Animator animator;
    public Collider2D colliderCuerpo;
    public Collider2D colliderEscudo;

    [Header("Vida")]
    public int vidaMaxima = 100;
    private int vidaActual;
    private bool muerto = false;

    [Header("Audio")]
    public AudioClip sonidoImpactoEscudo;
    public AudioClip sonidoAtaque;
    public AudioClip sonidoMuerte;

    [Header("Daño")]
    public int dañoAtaque = 1;

    public Collider2D hitboxMachete;
    public float rangoAtaque = 1.5f;
    public float rangoDeteccion = 5f;

    [Header("Referencias")]
    public Rigidbody2D rb;
    public GameObject escudoCaido;

    [Header("Movimiento y detección")]
    public float velocidad = 2f;

    private bool enRangoAtaque = false;
    private bool estaAtacando = false;
    private Transform jugador;
    private bool jugadorDetectado = false;

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;

        hitboxMachete.enabled = false;

        colliderEscudo.enabled = true;
        colliderCuerpo.enabled = false;
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

        if (muerto)
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

    public void ActivarHitbox()
    {
        hitboxMachete.enabled = true;
    }

    public void DesactivarHitbox()
    {
        hitboxMachete.enabled = false;
    }

    public void FinalizarAtaque()
    {
        estaAtacando = false;

        colliderEscudo.enabled = true;

        colliderCuerpo.enabled = false;
    }

    public void IniciarAtaque()
    {
        estaAtacando = true;
        colliderEscudo.enabled = false;
        colliderCuerpo.enabled = true;

        if (sonidoAtaque != null)
        AudioSource.PlayClipAtPoint(sonidoAtaque, transform.position);
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

    private void MoverHaciaJugador()
    {
        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0f;
        direccion.Normalize();

        transform.position += direccion * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitboxMachete.enabled && other.CompareTag("Player"))
        {
        }
        if (other.CompareTag("bala") && colliderEscudo.enabled)
        {
            Debug.Log("La bala impactó el ESCUDO");
            AudioSource.PlayClipAtPoint(sonidoImpactoEscudo, transform.position);

            Destroy(other.gameObject);
        }else if (other.CompareTag("bala") && !(colliderEscudo.enabled)){
            Debug.Log("La bala impactó al SOLDADO");

        }
       
    }

    public void RecibirDaño(int cantidad)
    {
        if (muerto) return;

        vidaActual -= cantidad;

        if (vidaActual <= 0)
            Morir();
    }

    private void Morir()
    {
        if (muerto) return;

        muerto = true;

        animator.SetBool("jugadorDetectado", false);
        animator.SetBool("enRangoAtaque", false);
        animator.SetBool("estaCaminando", false);

        escudoCaido.SetActive(true);

        colliderCuerpo.enabled = false;
        colliderEscudo.enabled = false;
        hitboxMachete.enabled = false;

        if (sonidoMuerte != null)
            AudioSource.PlayClipAtPoint(sonidoMuerte, transform.position);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        animator.SetBool("muerte", true);

        StopAllCoroutines();
        Destroy(gameObject, 2.5f);

        DropLoot drop = GetComponent<DropLoot>();
        if (drop != null)
            drop.SoltarObjetos();
    }


    
}