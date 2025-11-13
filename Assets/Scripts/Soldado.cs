using UnityEngine;

public class Soldado : MonoBehaviour
{
    [Header("Rangos")]
    [SerializeField] private float rangoLargo = 12f;
    [SerializeField] private float rangoMedio = 7f;
    [SerializeField] private float rangoCuerpo = 2f;

    [Header("Ataques")]
    [SerializeField] private float velocidadMovimiento = 2f;
    [SerializeField] private int dañoCuerpo = 20;

    [Header("Animaciones")]
    [SerializeField] private string animIdle = "Idle";
    [SerializeField] private string animDisparo = "DisparoyRecarga";
    [SerializeField] private string animDisparoAgachado = "DisparoyRecargaAgachado";
    [SerializeField] private string animCuerpo = "CuerpoACuerpo";
    [SerializeField] private string animCaminar = "Caminar";
    [SerializeField] private string animMuerte = "Muerte";

    [Header("Disparo")] // 🟩 NUEVO
    [SerializeField] private GameObject prefabBala; // 🟩 NUEVO
    [SerializeField] private Transform puntoDisparo; // 🟩 NUEVO
    [SerializeField] private float fuerzaDisparo = 8f; // 🟩 NUEVO

    private Transform jugador;
    private Animator anim;
    private Rigidbody2D rb;

    private bool mirandoDerecha = true;
    private bool muerto = false;
    private string animActual = "";

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.freezeRotation = true;
    }

    private void Update()
    {
        if (muerto || jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Voltear hacia el jugador
        bool debeMirarDerecha = (jugador.position.x > transform.position.x);
        if (debeMirarDerecha != mirandoDerecha)
            Voltear(debeMirarDerecha);

        // --- Comportamientos según distancia ---
        if (distancia > rangoLargo)
        {
            CambiarAnim(animIdle);
        }
        else if (distancia <= rangoLargo && distancia > rangoMedio)
        {
            CambiarAnim(animDisparo); // dispara de pie
        }
        else if (distancia <= rangoMedio && distancia > rangoCuerpo)
        {
            CambiarAnim(animDisparoAgachado); // dispara agachado
        }
        else if (distancia <= rangoCuerpo)
        {
            // acercarse al jugador
            Vector2 dir = (jugador.position - transform.position).normalized;
            rb.MovePosition(rb.position + dir * velocidadMovimiento * Time.deltaTime);

            // si ya está cerca, atacar cuerpo a cuerpo
            if (Vector2.Distance(transform.position, jugador.position) <= 1.5f)
                CambiarAnim(animCuerpo);
            else
                CambiarAnim(animCaminar);
        }
    }

    // ---------- UTILIDADES ----------

    private void CambiarAnim(string nuevaAnim)
    {
        if (animActual == nuevaAnim) return;
        anim.Play(nuevaAnim);
        animActual = nuevaAnim;
    }

    private void Voltear(bool mirarDerechaNuevo)
    {
        mirandoDerecha = mirarDerechaNuevo;
        Vector3 escala = transform.localScale;
        escala.x = mirarDerechaNuevo ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    // ---------- MUERTE ----------

    public void Morir()
    {
        if (muerto) return;
        muerto = true;
        CambiarAnim(animMuerte);
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
    }

    // ---------- DEBUG ----------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangoLargo);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoMedio);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoCuerpo);
    }
    private void Disparar()
    {
        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, puntoDisparo.rotation);

        Rigidbody2D rbBala = bala.GetComponent<Rigidbody2D>();
        if (rbBala != null)
        {
            // 🔁 Invertimos la dirección porque el sprite está al revés
            Vector2 direccion = mirandoDerecha ? Vector2.left : Vector2.right;
            rbBala.linearVelocity = direccion * fuerzaDisparo;
        }
    }
}
