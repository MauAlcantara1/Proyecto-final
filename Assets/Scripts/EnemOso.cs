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
    private Vector3 escalaOriginal; // escala guardada tal cual en el prefab
    private float baseScaleX;      // magnitud (abs) de escala X para mantener tamaño correcto

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

        // Inicializar mirandoDerecha según la escala original: si escala.x > 0 entonces el sprite "mira" derecha.
        mirandoDerecha = escalaOriginal.x > 0f;
        // Asegurar que la escala en X sea consistente con la variable 'direccion' y la orientación inicial
        // (esto evita inconsistencias si el prefab viene con escala.x negativa)
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

        // Detectar jugador
        if (d <= dDeteccion)
        {
            anim.SetBool("detEnem", true);

            // Si está lo suficientemente cerca, atacar
            if (d <= dAtaque)
            {
                anim.SetBool("PrepAtk", true);
                anim.SetBool("atqEnem", true);
                Ataque();

                // Luego del ataque puede repetir y agacharse
                StartCoroutine(RepetirCicloAtaque());
            }
            else
            {
                // Seguir al jugador caminando
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
            // Si no detecta jugador, patrulla
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

        if (transform.position.x > puntoInicial + rangoPatrulla)
            direccion = -1;
        else if (transform.position.x < puntoInicial - rangoPatrulla)
            direccion = 1;

        // Mantener el tamaño original y solo invertir la X según la dirección de patrulla
        transform.localScale = new Vector3(baseScaleX * direccion, escalaOriginal.y, escalaOriginal.z);

        // Sincronizar mirandoDerecha con la direccion de patrulla para coherencia visual
        mirandoDerecha = (direccion == 1);
    }

    private void GirarHaciaJugador()
    {
        if (jugador == null) return;

        // Decidir la dirección deseada: 1 si jugador está a la derecha, -1 si a la izquierda
        int direccionDeseada = jugador.position.x > transform.position.x ? 1 : -1;

        // Si la dirección visual actual no coincide con la deseada, actualizamos la escala
        bool actualmenteMirandoDerecha = mirandoDerecha;
        bool deberiaMirarDerecha = direccionDeseada == 1;

        if (actualmenteMirandoDerecha != deberiaMirarDerecha)
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
            // Si ya está por debajo del umbral, caer y huir
            anim.SetBool("Caer", true);
            anim.SetBool("Huir", true);
            enCombate = false;
            VidasPlayer.puntuacion += puntosPorMuerte;
            StartCoroutine(DestruirEnemigo());
        }
        else
        {
            // Si no muere, solo reacciona
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
