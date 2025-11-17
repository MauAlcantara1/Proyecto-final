using UnityEngine;
using System.Collections;

public class Soldadouno : MonoBehaviour
{
    [SerializeField] private AudioClip sfxDisparo;
    [SerializeField] private AudioSource audioSource;
    private bool estaMuerto = false;
    [SerializeField] private AudioClip sonidoMuerte;
    public float velocidadMovimiento = 3f;
    public float rangoDeteccion = 12f;
    public float rangoDisparoDist = 7f;
    public float rangoOrientacion = 3f;
    public float rangoCAC = 1.5f;
    private Coroutine rutinaDisparo = null;
    public GameObject prefabBala;
    public Transform puntoDisparo;
    public float fuerzaDisparo = 6f;
    public float tiempoEntreDisparos = 1.5f;
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;
    private Animator anim;
    private Rigidbody2D rb;
    private Transform jugador;
    private bool mirandoDerecha = true;
    private bool puedeDisparar = true;
    private bool muerto = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        vidaActual = vidaMaxima;

        rb.freezeRotation = true;
    }

    void Update()
    {
        if (jugador == null || muerto) return;
        ActualizarOrientacion();
        float dist = Vector2.Distance(transform.position, jugador.position);
        bool jugadorVisto = dist <= rangoDeteccion;
        bool rangoDisparo = dist <= rangoDisparoDist;
        bool enemigoCerca = dist <= rangoCAC;

        anim.SetBool("jugadorVisto", jugadorVisto);
        anim.SetBool("rangoDisparo", rangoDisparo);
        anim.SetBool("enemigoCerca", enemigoCerca);
        anim.SetBool("jugadorMuerto", false);

       
        if (enemigoCerca)
        {
            StopAllCoroutines();
            return;
        }

        
        if (rangoDisparo)
        {
            if (rutinaDisparo == null)
                rutinaDisparo = StartCoroutine(CicloDisparo());

            return;
        }
        else
        {
            if (rutinaDisparo != null)
            {
                StopCoroutine(rutinaDisparo);
                rutinaDisparo = null;
                puedeDisparar = true; 
            }
        }


        
        if (jugadorVisto)
        {
            MoverHaciaJugador();
        }
    }

    private void MoverHaciaJugador()
    {
        if (jugador == null) return;

        Vector2 dir = (jugador.position - transform.position).normalized;

        bool debeMirarDerecha = dir.x > 0;

        if (debeMirarDerecha != mirandoDerecha)
            Voltear(debeMirarDerecha);

        transform.position += (Vector3)(dir * velocidadMovimiento * Time.deltaTime);
    }


    private void Voltear(bool mirarDerechaNuevo)
    {
        mirandoDerecha = mirarDerechaNuevo;
        Vector3 escala = transform.localScale;
        escala.x = mirarDerechaNuevo ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
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
        if (estaMuerto) return;
        estaMuerto = true;

        if (audioSource != null && sonidoMuerte != null)
            audioSource.PlayOneShot(sonidoMuerte);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; 
        }

        anim.Play("muerte");
        anim.SetTrigger("muerte");


        StopAllCoroutines();

        DropLoot drop = GetComponent<DropLoot>();
        if (drop != null)
        {
            drop.SoltarObjetos();
            
        }
    }

    private IEnumerator CicloDisparo()
    {
        puedeDisparar = false;

        yield return new WaitForSeconds(tiempoEntreDisparos);

        puedeDisparar = true;
    }

    private void ActualizarOrientacion()
    {
        if (jugador == null) return;

        float distX = jugador.position.x - transform.position.x;

        if (Mathf.Abs(distX) <= rangoOrientacion)
        {
            bool debeMirarDerecha = distX > 0;

            if (debeMirarDerecha != mirandoDerecha)
                Voltear(debeMirarDerecha);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDisparoDist);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoCAC);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoOrientacion);

    }
    private void Disparar()
    {
        audioSource.PlayOneShot(sfxDisparo);

        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, puntoDisparo.rotation);

        Rigidbody2D rbBala = bala.GetComponent<Rigidbody2D>();

        Vector3 escala = bala.transform.localScale;
        escala.x = mirandoDerecha ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        bala.transform.localScale = escala;

        if (rbBala != null)
        {
            Vector2 direccion = mirandoDerecha ? Vector2.right : Vector2.left;
            rbBala.linearVelocity = direccion * fuerzaDisparo;
        }
    }

    private void Desaparecer()
    {
        Destroy(gameObject);
    }


}
