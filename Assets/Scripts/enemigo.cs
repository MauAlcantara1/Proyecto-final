using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    private Animator anim;
    public Rigidbody2D rb;
    public Transform jugador;
    private bool mirandoderecha = false;

    [Header("Vida")]
    public int vida_INI;
    private int vida;
    public GameObject barraVida;

    [Header("Ataque")]
    public Transform controladorAtaque;
    public float radioAtaque;
    public int dañoAtaque;
    public float tiempoEntreAtaques, tiempoEntreGiros;
    public float tiempoSigAtaque, tiempoSigGiro;
    public static float distanciaEnemigoJugador;
    [SerializeField] private float dAtaque, dSeguimiento, velSeguimiento;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        tiempoSigGiro = tiempoEntreGiros; // Empieza a girar para ver donde está el personaje
        vida = vida_INI;
    }

    void Update()
    {
        if (tiempoSigGiro > 0)
        {
            tiempoSigGiro -= Time.deltaTime;
        }
        else
        {
            MirarJugador();
            tiempoSigGiro = tiempoEntreGiros;
        }

        distanciaEnemigoJugador = Vector2.Distance(transform.position, jugador.position);
        anim.SetFloat("DistanciaJugador", distanciaEnemigoJugador);

        //Determina que accion realizar en contra del jugador
        DeterminaAccion(distanciaEnemigoJugador);
    }

private void DeterminaAccion(float d)
{
        ResetAnimsEnemigo();
        if (d <= radioAtaque)
        {
            // Enemigo ataca
            Ataque();
            anim.SetBool("atacando", true);
        }

        else if (d > radioAtaque && d <= dSeguimiento)
        {
            // Seguir al player
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector2(jugador.position.x, transform.position.y),
                velSeguimiento / 1000
            );
        }
        else
        {
            // Estático
        }
}
private void ResetAnimsEnemigo()
{
    anim.SetBool("atacando", false);
    anim.SetBool("caminando", false);
}


    public void TomarDaño(int daño)
    {
        vida -= daño;
        DibujaBarra(vida);

        if (vida <= 0)
        {
            anim.SetTrigger("Muerte");
            StartCoroutine(EliminaEnemigo());
        }
    }

    IEnumerator EliminaEnemigo()
    {
        yield return new WaitForSeconds(1.2f); // Anim de 1.2 segs antes de que lo destruyamos
        Muerte();
    }

    private void Muerte()
    {
        Destroy(this.gameObject);
    }

    public void MirarJugador()
    {
        if ((jugador.position.x > transform.position.x && !mirandoderecha) ||
            (jugador.position.x < transform.position.x && mirandoderecha))
        {
            mirandoderecha = !mirandoderecha;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        }
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

    private void DibujaBarra(float n)
    {
        barraVida.transform.localScale = new Vector2(0.73f * n / vida_INI, barraVida.transform.localScale.y);
    }
}
