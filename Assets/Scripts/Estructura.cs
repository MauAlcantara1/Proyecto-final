using UnityEngine;

public class Estructura : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 30;  
    [SerializeField]private int vidaActual;

    [Header("Componentes")]
    private Animator animator;
    private Collider2D col;

    private bool estado1Activado = false;
    private bool estado2Activado = false;
    private bool estado3Activado = false;

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
        if (estado3Activado) return;

        vidaActual -= cantidad;
        Debug.Log("Daño recibido. Vida restante: " + vidaActual);

        if (vidaActual <= 20 && !estado1Activado)
        {
            estado1Activado = true;
            animator.SetBool("activoEstado1", true);
        }

        if (vidaActual <= 10 && !estado2Activado)
        {
            estado2Activado = true;
            animator.SetBool("activoEstado2", true);
        }
        if (vidaActual <= 0 && !estado3Activado)
        {
            Destroy(gameObject);
        }
    }
}
