using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private Transform jugador;
    private bool mirandoderecha = false;

    [Header("Vida")]
    public int vida_INI = 3;
    private int vida;

    [Header("Ataque")]
    public Transform controladorAtaque;
    public float radioAtaque = 0.5f;
    public int dañoAtaque = 1;
    [SerializeField] private float dSeguimiento = 6f, velSeguimiento = 2f;

    [Header("Patrullaje")]
    public float rangoPatrulla = 4f; // Rango máximo a cada lado
    private float puntoInicial;
    private int direccion = 1; // 1 = derecha, -1 = izquierda

    [Header("Puntuación")]
    public int puntosPorMuerte = 100; // 👈 Editable en el Inspector

    public static float distanciaEnemigoJugador;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        vida = vida_INI;
        puntoInicial = transform.position.x;
    }

    void Update()
    {
        distanciaEnemigoJugador = Vector2.Distance(transform.position, jugador.position);
        anim.SetFloat("DistanciaJugador", distanciaEnemigoJugador);
        DeterminaAccion(distanciaEnemigoJugador);
    }

    private void DeterminaAccion(float d)
    {
        ResetAnimsEnemigo();

        if (d <= radioAtaque)
        {
            Ataque();
            anim.SetBool("atacando", true);
        }
        else if (d > radioAtaque && d <= dSeguimiento)
        {
            // Seguir al jugador
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector2(jugador.position.x, transform.position.y),
                velSeguimiento * Time.deltaTime
            );

            // 🔁 Girar hacia el jugador
            if ((jugador.position.x > transform.position.x && !mirandoderecha) ||
                (jugador.position.x < transform.position.x && mirandoderecha))
            {
                Girar();
            }

            anim.SetBool("caminando", true);
        }
        else
        {
            // Patrullar cuando no detecta al jugador
            Patrullar();
        }
    }

    private void Patrullar()
    {
        anim.SetBool("caminando", true);
        transform.Translate(Vector2.right * direccion * velSeguimiento * Time.deltaTime);

        if (transform.position.x >= puntoInicial + rangoPatrulla)
        {
            direccion = -1;
            Girar();
        }
        else if (transform.position.x <= puntoInicial - rangoPatrulla)
        {
            direccion = 1;
            Girar();
        }
    }

    private void Girar()
    {
        mirandoderecha = !mirandoderecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;   // 👈 invierte el eje X sin afectar rotación
        transform.localScale = escala;
    }

    private void ResetAnimsEnemigo()
    {
        anim.SetBool("atacando", false);
        anim.SetBool("caminando", false);
    }

    public void TomarDaño(int daño)
    {
        vida -= daño;

        if (vida <= 0)
        {
            anim.SetTrigger("Muerte");
            VidasPlayer.puntuacion += puntosPorMuerte;
            StartCoroutine(EliminaEnemigo());
        }
    }

    IEnumerator EliminaEnemigo()
    {
        yield return new WaitForSeconds(1.2f);
        Destroy(this.gameObject);
    }

    private void Ataque()
    {
        Collider2D[] objs = Physics2D.OverlapCircleAll(controladorAtaque.position, radioAtaque);
        foreach (Collider2D colisionador in objs)
        {
            if (colisionador.CompareTag("Player"))
            {
                colisionador.transform.GetComponent<VidasPlayer>().TomarDaño(dañoAtaque);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (controladorAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(controladorAtaque.position, radioAtaque);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            new Vector2(transform.position.x - rangoPatrulla, transform.position.y),
            new Vector2(transform.position.x + rangoPatrulla, transform.position.y)
        );
    }
}
