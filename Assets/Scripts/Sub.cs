using UnityEngine;

public class Sub : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 20;
    private int vidaActual;

    [Header("Componentes")]
    private Animator animator;
    private Collider2D col;

    // Para evitar activar animación de estado 1 más de una vez
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

        // --- Estado 1 al llegar a 10 ---
        if (vidaActual <= 10 && !estado1Activado)
        {
            estado1Activado = true;
            animator.SetBool("activoEstado1", true);
        }

        // Si quieres que al llegar a 0 pase algo, dímelo
    }
}
