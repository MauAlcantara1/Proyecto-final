using UnityEngine;

public class Tanque : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float distanciaDeteccion = 15f;
    [SerializeField] private float distanciaFrenado = 10f;

    private Animator animator;
    private Transform player;

    private bool persiguiendo = false;

    private bool arranqueOriginalHecho = false;
    private bool arranqueOriginalTerminado = false;

    private bool frenando = false;
    private bool prepHecho = false;
    private bool disparoHecho = false;

    private bool arranque0Activo = false;
    private bool arranque0Terminado = false;

    private bool enProcesoAtaque = false;

    private Vector3 puntoInicial;
    private Vector3 puntoFinal;
    private bool yendoAlFinal = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        puntoInicial = transform.position;
        puntoFinal = puntoInicial + transform.forward * 4f;

        Debug.Log("✅ Tanque iniciado.");
    }

    private void Update()
    {
        DetectarJugador();
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        // ✅ ARRANQUE ORIGINAL → Movimiento
        if (arranqueOriginalHecho && !arranqueOriginalTerminado)
        {
            if (info.IsName("Arranque") && info.normalizedTime >= 0.95f)
            {
                arranqueOriginalTerminado = true;
                animator.SetTrigger("Mover");
                Debug.Log("✅ Arranque original terminado → Movimiento0.");
            }
        }

        // ✅ ARRANQUE0 → Movimiento1
        if (arranque0Activo && !arranque0Terminado)
        {
            if (info.IsName("Arranque0") && info.normalizedTime >= 0.95f)
            {
                arranque0Terminado = true;
                animator.SetTrigger("Mover1");
                Debug.Log("✅ Arranque0 terminado → Movimiento0 (Mover1).");
            }
        }

        // ✅ FRENADO → Prep
        if (frenando && !prepHecho)
        {
            if (info.IsName("Frenado") && info.normalizedTime >= 0.95f)
            {
                prepHecho = true;
                enProcesoAtaque = true;
                animator.ResetTrigger("Frenado");
                animator.SetTrigger("Preparacion");
                Debug.Log("✅ Frenado terminado → Preparacion.");
            }
        }

        // ✅ PREP → Disparo
        if (prepHecho && !disparoHecho)
        {
            if (info.IsName("Prep") && info.normalizedTime >= 0.95f)
            {
                disparoHecho = true;
                enProcesoAtaque = true;
                animator.ResetTrigger("Preparacion");
                animator.SetTrigger("Disparo");
                Debug.Log("✅ Preparacion terminada → Disparo.");
            }
        }

        // ✅ DISPARO → Arranque1 (reinicio de movimiento)
        if (disparoHecho)
        {
            if (info.IsName("Disparo") && info.normalizedTime >= 0.95f)
            {
                Debug.Log("🏁 Disparo terminado → reiniciando ciclo de movimiento con Arranque1.");

                arranque0Activo = true;
                arranque0Terminado = false;

                frenando = false;
                prepHecho = false;
                disparoHecho = false;
                enProcesoAtaque = false;

                animator.ResetTrigger("Disparo");
                animator.SetTrigger("Arranque1");
            }
        }

        // ✅ MOVIMIENTO O PERSECUCIÓN
        // Eliminamos dependencia de nombres exactos de animaciones, usamos lógica general
        if (!enProcesoAtaque && persiguiendo && (arranqueOriginalTerminado || arranque0Terminado))
        {
            float distancia = Vector3.Distance(transform.position, player.position);

            if (distancia <= distanciaFrenado)
            {
                frenando = true;
                enProcesoAtaque = true;
                animator.SetTrigger("Frenado");
                Debug.Log("🛑 Jugador en rango de ataque → Frenando.");
                return;
            }

            // Se mueve siempre que el Animator no esté en una animación de ataque o arranque
            if (!info.IsName("Arranque") &&
                !info.IsName("Arranque0") &&
                !info.IsName("Frenado") &&
                !info.IsName("Prep") &&
                !info.IsName("Disparo"))
            {
                PerseguirJugador();
                Debug.Log("🚗 Persiguiendo al jugador...");
            }
        }
        else if (!persiguiendo && !enProcesoAtaque)
        {
            Patrullar();
        }
    }

    private void DetectarJugador()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= distanciaDeteccion)
        {
            persiguiendo = true;

            if (!arranqueOriginalHecho)
            {
                arranqueOriginalHecho = true;
                arranqueOriginalTerminado = false;
                animator.SetTrigger("Arranque");
                Debug.Log("🚀 Activando Arranque inicial.");
            }
        }
        else
        {
            persiguiendo = false;

            if (!enProcesoAtaque)
            {
                arranque0Activo = false;
                arranque0Terminado = false;
                frenando = false;
                prepHecho = false;
                disparoHecho = false;

                animator.ResetTrigger("Arranque");
                animator.ResetTrigger("Arranque1");
                animator.ResetTrigger("Frenado");
                animator.ResetTrigger("Preparacion");
                animator.ResetTrigger("Disparo");

                Debug.Log("⚪ Jugador fuera de detección → reseteando estado.");
            }
            else
            {
                Debug.Log("⚪ Jugador fuera de detección, pero hay ataque en curso (no interrumpir).");
            }
        }
    }

    private void PerseguirJugador()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            velocidad * Time.deltaTime
        );
    }

    private void Patrullar()
    {
        if (arranqueOriginalHecho) return;

        Vector3 objetivo = yendoAlFinal ? puntoFinal : puntoInicial;
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);

        if (Vector3.Distance(transform.position, objetivo) < 0.1f)
            yendoAlFinal = !yendoAlFinal;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaFrenado);
    }
}
