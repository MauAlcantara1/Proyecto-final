using UnityEngine;

public class Estructura : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 20;   // ✔ Cambiado a 20
    private int vidaActual;

    [Header("Componentes")]
    private Animator animator;
    private Collider2D col;

    // Para evitar avances repetidos
    private bool estado1Activado = false;
    private bool estado2Activado = false;

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
        if (estado2Activado) return;

        vidaActual -= cantidad;
        Debug.Log("Daño recibido. Vida restante: " + vidaActual);

        // --- Estado 1 al llegar a 10 ---
        if (vidaActual <= 10 && !estado1Activado)
        {
            estado1Activado = true;
            animator.SetBool("activoEstado1", true);
        }

        // --- Estado 2 al llegar a 0 ---
        if (vidaActual <= 0 && !estado2Activado)
        {
            estado2Activado = true;
            animator.SetBool("activoEstado2", true);
            col.enabled = false;  // Desactiva collider
        }
    }
}
