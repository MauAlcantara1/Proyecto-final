using UnityEngine;

public class Reloj : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 20;
    private int vidaActual;

    [Header("Componentes")]
    private Animator animator;
    private Collider2D col;

    // Evita activar el estado más de una vez
    private bool estado1Activado = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        vidaActual = vidaMaxima;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger con: " + other.tag);

        if (other.CompareTag("bala"))
        {
            RecibirDaño(1);
        }
    }

    public void RecibirDaño(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("Daño recibido. Vida restante: " + vidaActual);

        // --- Activar animación "reloj" ---
        if (vidaActual <= 10 && !estado1Activado)
        {
            estado1Activado = true;
            animator.SetBool("activarEstado1", true); // idle → reloj
        }
    }
}
