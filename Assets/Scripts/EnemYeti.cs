using UnityEngine;

public class EnemYeti : MonoBehaviour
{
    [SerializeField] private float rangoDeteccion = 18f;
    [SerializeField] private float rangoAtaque = 3f;
    [SerializeField] private float velocidadMovimiento = 2f;

    [SerializeField] private int vidaMaxima = 100;

    [SerializeField] private string animIdle = "Idle";
    [SerializeField] private string animCaminar = "Caminar";
    [SerializeField] private string animAtacar = "Ataque";
    [SerializeField] private string animMuerte = "Muerte";

    private int vidaActual;
    private bool mirandoDerecha;
    private bool jugadorDetectado = false;
    private bool atacando = false;
    private bool muerto = false;


    private Transform jugador;
    private Transform jugador1;
    private Transform jugador2;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private AudioSource audioSource;

    private void Start()
    {

        GameObject j1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject j2 = GameObject.FindGameObjectWithTag("Player2");

        if (j1 != null) jugador1 = j1.transform;
        if (j2 != null) jugador2 = j2.transform;

        jugador = ObtenerJugadorObjetivo();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();


        if (rb != null)
            rb.freezeRotation = true;

        vidaActual = vidaMaxima;

        mirandoDerecha = transform.localScale.x > 0;
    }


    private void Update()
    {
        jugador = ObtenerJugadorObjetivo();
        if (jugador == null || muerto) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);
        jugadorDetectado = distancia <= rangoDeteccion;

        if (!jugadorDetectado)
        {
            anim.Play(animIdle);
            return;
        }

        if (distancia <= rangoAtaque && !atacando)
        {
            anim.Play(animAtacar);
            atacando = true;
        }
        else if (distancia > rangoAtaque && !atacando)
        {
            MoverHaciaJugador();
        }
    }

    private void MoverHaciaJugador()
    {
        if (jugador == null) return;

        Vector2 dir = (jugador.position - transform.position).normalized;
        bool debeMirarDerecha = dir.x < 0;

        if (debeMirarDerecha != mirandoDerecha)
            Voltear(debeMirarDerecha);

        anim.Play(animCaminar);
        transform.position += (Vector3)(dir * velocidadMovimiento * Time.deltaTime);
    }

    private void Voltear(bool mirarDerechaNuevo)
    {
        mirandoDerecha = mirarDerechaNuevo;
        Vector3 escala = transform.localScale;
        escala.x = mirarDerechaNuevo ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
    }


    public void TerminarAtaque()
    {
        atacando = false;
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
        if (audioSource != null)
            audioSource.Play();

        muerto = true;
        anim.Play(animMuerte);

        

        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; 
        }


        GetComponent<Collider2D>().enabled = false;
        StopAllCoroutines();

        DropLoot drop = GetComponent<DropLoot>();
        if (drop != null)
            drop.SoltarObjetos();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
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
