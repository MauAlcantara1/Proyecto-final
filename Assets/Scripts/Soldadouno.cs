using UnityEngine;

public class Soldadouno : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 3f;
    public float rangoPerseguir = 12f;     // Rango 1
    public float rangoDisparo = 7f;        // Rango 2
    public float rangoCAC = 1.5f;          // Rango 3

    [Header("Disparo")]
    public GameObject prefabBala;
    public Transform puntoDisparo;
    public float fuerzaDisparo = 6f;
    public float tiempoEntreDisparos = 1.5f;
    private float proximoDisparo = 0f;

    private Animator anim;
    private Transform player;
    private bool mirandoDerecha = true;

    // Alternancia entre disparo agachado o parado
    private bool disparoAgachado = false;

    // Control interno
    private bool enDisparo = false;
    private bool enCAC = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        MirarJugador();

        // -------------------------------
        // 🟥 RANGO 3 – CUERPO A CUERPO
        // -------------------------------
        if (dist <= rangoCAC)
        {
            enDisparo = false;
            AtaqueCAC();
            return;
        }

        // -------------------------------
        // 🟧 RANGO 2 – DETENER Y DISPARAR
        // -------------------------------
        if (dist <= rangoDisparo)
        {
            enCAC = false;
            DispararSegunModo();
            return;
        }

        // -------------------------------
        // 🟩 RANGO 1 – PERSEGUIR
        // -------------------------------
        if (dist <= rangoPerseguir)
        {
            enDisparo = false;
            enCAC = false;
            Perseguir();
            return;
        }

        // 🟦 Fuera de rangos → Idle
        anim.Play("idle");
    }

    // ============================================================
    //  PERSEGUIR
    // ============================================================
    private void Perseguir()
    {
        anim.Play("caminar");

        Vector3 target = new Vector3(player.position.x, transform.position.y, 0);
        transform.position = Vector3.MoveTowards(transform.position, target, velocidad * Time.deltaTime);
    }

    // ============================================================
    //  DISPARO
    // ============================================================
    private void DispararSegunModo()
    {
        if (!enDisparo)
        {
            enDisparo = true;

            // Alternar entre disparo parado y agachado
            disparoAgachado = Random.value > 0.5f;
        }

        if (disparoAgachado)
            DisparoAgachado();
        else
            DisparoParado();
    }

    // ---- DISPARO PARADO ----
    private void DisparoParado()
    {
        anim.Play("disparoyrecarga");

        if (Time.time >= proximoDisparo)
        {
            InstanciarBala();
            proximoDisparo = Time.time + tiempoEntreDisparos;
        }
    }

    // ---- DISPARO AGACHADO ----
    private void DisparoAgachado()
    {
        // 1) Animación de agacharse antes del disparo
        anim.Play("agachado");

        if (Time.time >= proximoDisparo)
        {
            // 2) Animación disparo agachado
            anim.Play("disparoyrecargaagachado");

            InstanciarBala();

            // 3) Al terminar, levantarse
            anim.Play("levantarse");

            proximoDisparo = Time.time + tiempoEntreDisparos;
        }
    }

    // ============================================================
    //  ATAQUE CUERPO A CUERPO
    // ============================================================
    private void AtaqueCAC()
    {
        if (!enCAC)
        {
            enCAC = true;
            anim.Play("cac");
        }
    }

    // ============================================================
    //  MIRAR AL JUGADOR
    // ============================================================
    private void MirarJugador()
    {
        if (player == null) return;

        bool mirarDer = player.position.x > transform.position.x;

        if (mirarDer != mirandoDerecha)
        {
            mirandoDerecha = mirarDer;

            Vector3 esc = transform.localScale;
            esc.x *= -1;
            transform.localScale = esc;

            // invertir punto de disparo
            Vector3 p = puntoDisparo.localPosition;
            p.x *= -1;
            puntoDisparo.localPosition = p;
        }
    }

    // ============================================================
    //  CREAR BALA
    // ============================================================
    private void InstanciarBala()
    {
        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, Quaternion.identity);

        Rigidbody2D rb = bala.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 dir = mirandoDerecha ? Vector2.right : Vector2.left;
            rb.linearVelocity = dir * fuerzaDisparo;
        }
    }

    // ============================================================
    //  DIBUJAR RANGOS
    // ============================================================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangoPerseguir);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDisparo);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoCAC);
    }
}
