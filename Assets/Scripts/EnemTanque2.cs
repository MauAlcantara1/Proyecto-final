using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemTanque2 : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private Transform jugador;
    private bool mirandoderecha = true;

    [Header("Vida")]
    public int vida_INI = 3;
    private int vida;

    [Header("Ataque")]
    public Transform controladorAtaque;
    public float radioAtaque = 0.5f;
    public int dañoAtaque = 1;
    [SerializeField] private float dSeguimiento = 6f, velSeguimiento = 2f;

    [Header("Patrullaje")]
    public float rangoPatrulla = 4f;
    private float puntoInicial;
    private int direccion = 1;

    [Header("Puntuación")]
    public int puntosPorMuerte = 100;

    public static float distanciaEnemigoJugador;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        vida = vida_INI;
        puntoInicial = transform.position.x;
    }

    void Update()
    {
        distanciaEnemigoJugador = Vector2.Distance(transform.position, jugador.position);
        DeterminaAccion(distanciaEnemigoJugador);
    }

    private void DeterminaAccion(float d)
    {
        ResetAnimaciones();

        // --- Detección ---
        if (d <= dSeguimiento)
        {
            anim.SetBool("DetEnem", true);
        }

        // --- Ataque cercano ---
        if (d <= radioAtaque)
        {
            anim.SetBool("DecidirATK", true);
            anim.SetBool("DispEnem", true);
            Ataque();
        }
        // --- Movimiento hacia el jugador ---
        else if (d > radioAtaque && d <= dSeguimiento)
        {
            anim.SetBool("Cargar", true);

            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector2(jugador.position.x, transform.position.y),
                velSeguimiento * Time.deltaTime
            );

            GirarHaciaJugador();
        }
        // --- Patrulla si el jugador está lejos ---
        else
        {
            Patrullar();
        }
    }

    private void Patrullar()
    {
        anim.SetBool("Volver a disp", true);
        transform.Translate(Vector2.right * direccion * velSeguimiento * Time.deltaTime);

        if (transform.position.x > puntoInicial + rangoPatrulla)
            direccion = -1;
        else if (transform.position.x < puntoInicial - rangoPatrulla)
            direccion = 1;

        // Gira visualmente al patrullar
        transform.localScale = new Vector3(direccion, 1, 1);
    }

    private void GirarHaciaJugador()
    {
        if (jugador == null) return;

        if ((jugador.position.x > transform.position.x && !mirandoderecha) ||
            (jugador.position.x < transform.position.x && mirandoderecha))
        {
            mirandoderecha = !mirandoderecha;
            transform.Rotate(0f, 180f, 0f);
            anim.SetBool("Girar", true);
        }
    }

    private void ResetAnimaciones()
    {
        anim.SetBool("DetEnem", false);
        anim.SetBool("DecidirATK", false);
        anim.SetBool("Embestir", false);
        anim.SetBool("Cargar", false);
        anim.SetBool("DispEnem", false);
        anim.SetBool("Volver a disp", false);
        anim.SetBool("Girar", false);
    }

    public void TomarDaño(int daño)
    {
        vida -= daño;
        anim.SetBool("Daño", true);

        if (vida <= 0)
        {
            anim.SetBool("Muerte", true);
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
        foreach (Collider2D col in objs)
        {
            if (col.CompareTag("Player"))
            {
                col.GetComponent<VidasPlayer>().TomarDaño(dañoAtaque);
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
