using UnityEngine;

public class EnemSoldado : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Rangos de acción (editables)")]
    public float rangoCuerpoACuerpo = 2.0f;
    public float rangoDisparo = 6.0f;
    public float rangoAgacharse = 9.0f;

    [Header("Atributos")]
    public float velocidad = 2.2f;
    public int danoDisparo = 5;
    public int danoCuerpoACuerpo = 10;
    public bool spriteMiraDerecha = true;

    private bool atacando = false;
    private bool disparando = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) jugador = playerObj.transform;
        }

        animator.Play("idle");
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // --- Voltear sprite según posición del jugador ---
        bool mirarIzquierda = (jugador.position.x < transform.position.x);
        spriteRenderer.flipX = spriteMiraDerecha ? mirarIzquierda : !mirarIzquierda;

        // --- Resetea animaciones si no hay acción ---
        animator.SetBool("enemigcerc", false);
        animator.SetBool("rango", false);
        animator.SetBool("agachadodisparar", false);
        animator.SetBool("cuerpoacuerpo", false);

        // --- Decisiones según distancia ---
        if (distancia <= rangoCuerpoACuerpo)
        {
            // Cuerpo a cuerpo
            animator.SetBool("enemigcerc", true);
            MoverHaciaJugador();

            if (distancia < (rangoCuerpoACuerpo - 0.2f))
                AtacarCuerpoACuerpo();
        }
        else if (distancia <= rangoDisparo)
        {
            // Disparo normal
            animator.SetBool("rango", true);
            Disparar();
        }
        else if (distancia <= rangoAgacharse)
        {
            // Disparo agachado
            animator.SetBool("agachadodisparar", true);
            DispararAgachado();
        }
        else
        {
            // Idle
            animator.Play("idle");
        }
    }

    private void MoverHaciaJugador()
    {
        if (jugador == null) return;

        Vector3 dir = (jugador.position - transform.position).normalized;
        transform.position += dir * velocidad * Time.deltaTime;
    }

    private void Disparar()
    {
        if (disparando) return;
        disparando = true;

        animator.SetBool("rango", true);
        Debug.Log("[Soldado] 🔫 Disparo normal: -" + danoDisparo + " HP");

        Invoke(nameof(FinDisparo), 1.2f);
    }

    private void DispararAgachado()
    {
        if (disparando) return;
        disparando = true;

        animator.SetBool("agachadodisparar", true);
        Debug.Log("[Soldado] 🔫 Disparo agachado: -" + danoDisparo + " HP");

        Invoke(nameof(FinDisparo), 1.2f);
    }

    private void FinDisparo()
    {
        disparando = false;
        animator.SetBool("rango", false);
        animator.SetBool("agachadodisparar", false);
    }

    private void AtacarCuerpoACuerpo()
    {
        if (atacando) return;
        atacando = true;

        animator.SetBool("cuerpoacuerpo", true);
        Debug.Log("[Soldado] 💥 Golpe cuerpo a cuerpo: -" + danoCuerpoACuerpo + " HP");

        Invoke(nameof(FinGolpe), 0.8f);
    }

    private void FinGolpe()
    {
        atacando = false;
        animator.SetBool("cuerpoacuerpo", false);
    }
}
