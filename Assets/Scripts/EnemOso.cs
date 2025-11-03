using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemOso : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private Transform jugador;
    private bool mirandoDerecha;
    private bool enCombate = true;

    [Header("Vida")]
    public int vida_INI = 5;
    private int vida;
    public int dañoUmbral = 2; // Si vida <= este valor => Caer + Huir

    [Header("Ataque")]
    public Transform controladorAtaque;
    public float radioAtaque = 0.7f;
    public int dañoAtaque = 1;
    [SerializeField] private float dDeteccion = 8f, dAtaque = 1.5f, velMovimiento = 2f;

    [Header("Patrullaje")]
    public float rangoPatrulla = 4f;
    private float puntoInicial;
    private int direccion = 1; // +1: derecha, -1: izquierda (para movimiento de patrulla)
    private Vector3 escalaOriginal;
    private float baseScaleX;

    [Header("Puntuación")]
    public int puntosPorMuerte = 150;
    public static float distanciaJugador;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        vida = vida_INI;
        puntoInicial = transform.position.x;
        escalaOriginal = transform.localScale;
        baseScaleX = Mathf.Abs(escalaOriginal.x);

        mirandoDerecha = escalaOriginal.x > 0f;
        float initialDir = mirandoDerecha ? 1f : -1f;
        transform.localScale = new Vector3(baseScaleX * initialDir, escalaOriginal.y, escalaOriginal.z);
    }

    void Update()
    {
        if (jugador == null || !enCombate) return;

        distanciaJugador = Vector2.Distance(transform.position, jugador.position);
        ControlarAccion(distanciaJugador);
    }

    private void ControlarAccion(float d)
    {
        ResetAnimaciones();

        if (d <= dDeteccion)
        {
            anim.SetBool("detEnem", true);

            if (d <= dAtaque)
            {
                anim.SetBool("PrepAtk", true);
                anim.SetBool("atqEnem", true);
                Ataque();
                StartCoroutine(RepetirCicloAtaque());
            }
            else
            {
                anim.SetBool("PrepCaminar", true);
                anim.SetBool("Avanzar", true);
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    new Vector2(jugador.position.x, transform.position.y),
                    velMovimiento * Time.deltaTime
                );
                GirarHaciaJugador();
            }
        }
        else
        {
            Patrullar();
        }
    }

    IEnumerator RepetirCicloAtaque()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("RepAtk", true);
        yield return new WaitForSeconds(0.8f);
        anim.SetBool("Agacharse", true);
        yield return new WaitForSeconds(0.8f);
        anim.SetBool("PrepCaminar", true);
        anim.SetBool("Avanzar", true);
    }

    private void Patrullar()
    {
        anim.SetBool("PrepCaminar", true);
        transform.Translate(Vector2.right * direccion * velMovimiento * Time.deltaTime);

        if (transform.position.x > puntoInicial + rangoPatrulla) direccion = -1;
        else if (transform.position.x < puntoInicial - rangoPatrulla) direccion = 1;

        transform.localScale = new Vector3(baseScaleX * direccion, escalaOriginal.y, escalaOriginal.z);
        mirandoDerecha = (direccion == 1);
    }

    private void GirarHaciaJugador()
    {
        if (jugador == null) return;

        int direccionDeseada = jugador.position.x > transform.position.x ? 1 : -1;
        bool deberiaMirarDerecha = direccionDeseada == 1;

        if (mirandoDerecha != deberiaMirarDerecha)
        {
            mirandoDerecha = deberiaMirarDerecha;
            transform.localScale = new Vector3(baseScaleX * direccionDeseada, escalaOriginal.y, escalaOriginal.z);
        }
    }

    private void ResetAnimaciones()
    {
        anim.SetBool("detEnem", false);
        anim.SetBool("PrepCaminar", false);
        anim.SetBool("Avanzar", false);
        anim.SetBool("PrepAtk", false);
        anim.SetBool("atqEnem", false);
        anim.SetBool("RepAtk", false);
        anim.SetBool("Agacharse", false);
        anim.SetBool("Caer", false);
        anim.SetBool("Huir", false);
    }

    public void TomarDaño(int daño)
    {
        vida -= daño;

        if (vida <= dañoUmbral)
        {
            anim.SetBool("Caer", true);
            anim.SetBool("Huir", true);
            enCombate = false;
            VidasPlayer.puntuacion += puntosPorMuerte;
            StartCoroutine(DestruirEnemigo());
        }
        else
        {
            StartCoroutine(ReaccionarAlDaño());
        }
    }

    IEnumerator ReaccionarAlDaño()
    {
        anim.SetBool("Agacharse", true);
        yield return new WaitForSeconds(0.6f);
        anim.SetBool("PrepCaminar", true);
        anim.SetBool("Avanzar", true);
    }

    IEnumerator DestruirEnemigo()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
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
