using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimarPorDistancia : MonoBehaviour
{
    public Transform jugador;
    public float distanciaActivacion = 5f;
    public string nombreAnimacion = "Tenkiu1"; // El nombre exacto del estado en el Animator

    private Animator anim;
    private bool animacionReproducida = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (jugador == null)
        {
            GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
            if (objJugador != null)
                jugador = objJugador.transform;
        }

        // Pausar la animación al inicio
        anim.speed = 0f;
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Si el jugador está dentro del rango y la animación aún no ha sido reproducida
        if (distancia <= distanciaActivacion && !animacionReproducida)
        {
            anim.Play(nombreAnimacion, 0, 0f); // Reproduce desde el inicio
            anim.speed = 1f; // Reactiva el movimiento de la animación
            animacionReproducida = true;
        }
        else if (distancia > distanciaActivacion + 2f)
        {
            // Si se aleja, detiene la animación y puede volver a activarse
            anim.speed = 0f;
            animacionReproducida = false;
        }
    }
}
