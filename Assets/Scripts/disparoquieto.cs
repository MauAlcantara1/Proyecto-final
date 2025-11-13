using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class disparoquieto : MonoBehaviour
{
    [Header("Detección del jugador")]
    public Transform jugador;
    public float rangoDisparo = 5f;          // Dispara si está a esta distancia
    public float rangoCuerpoACuerpo = 1.5f;  // Golpea si está muy cerca

    [Header("Movimiento")]
    public float velocidad = 2.5f;
    public bool spriteMiraDerecha = true;

    [Header("Daño")]
    public int danoCuerpoACuerpo = 10;
    public int danoDisparo = 5;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Estados
    private bool atacando = false;
    private bool disparando = false;
    private bool jugadorMuerto = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                jugador = playerObj.transform;
        }
    }

    private void Update()
    {
        if (jugador == null || jugadorMuerto) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Dirección visual del sprite
        bool flipDeseado = spriteMiraDerecha ? (jugador.position.x < transform.position.x) : (jugador.position.x > transform.position.x);
        spriteRenderer.flipX = flipDeseado;

        // Reset básico
        animator.SetBool("enemigcerc", false);
        animator.SetBool("rango", false);
        animator.SetBool("levantarse", false);

        // --- COMPORTAMIENTO ---
        if (distancia <= rangoCuerpoACuerpo)
        {
            // Ataque cuerpo a cuerpo (avanza un poco si no está pegado)
            MoverHaciaJugador(true);
            AtacarCuerpoACuerpo();
        }
        else if (distancia <= rangoDisparo)
        {
            // Disparo de pie (quieto)
            Disparar();
        }
        else
        {
            // Idle
            animator.SetBool("levantarse", true);
        }
    }

    private void MoverHaciaJugador(bool acercando)
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);
        if (acercando && distancia > rangoCuerpoACuerpo * 0.9f)
        {
            transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidad * Time.deltaTime);
            animator.SetBool("enemigcerc", true);
        }
        else
        {
            animator.SetBool("enemigcerc", false);
        }
    }

    private void AtacarCuerpoACuerpo()
    {
        if (atacando) return;

        atacando = true;
        animator.SetBool("cuerpoacuerpo", true);
        Debug.Log("[Soldado] Golpe cuerpo a cuerpo → -" + danoCuerpoACuerpo + " HP");

        Invoke(nameof(ReiniciarAtaque), 1.0f);
    }

    private void Disparar()
    {
        if (disparando || atacando) return;

        disparando = true;
        animator.SetBool("rango", true);
        Debug.Log("[Soldado] Disparo de pie → -" + danoDisparo + " HP");

        Invoke(nameof(ReiniciarDisparo), 1.2f);
    }

    private void ReiniciarAtaque()
    {
        atacando = false;
        animator.SetBool("cuerpoacuerpo", false);
    }

    private void ReiniciarDisparo()
    {
        disparando = false;
        animator.SetBool("rango", false);
    }

    public void JugadorMuerto()
    {
        jugadorMuerto = true;
        animator.SetBool("jugMuer", true);
        Debug.Log("[Soldado] Jugador eliminado → festeja");
    }
}
