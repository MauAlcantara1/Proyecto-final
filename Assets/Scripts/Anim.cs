using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimarPorDistancia : MonoBehaviour
{
    public Transform jugador;
    public Transform jugador1;
    public Transform jugador2;
    public float distanciaActivacion = 2f;
    public string nombreAnimacion = "Tenkiu1"; // El nombre exacto del estado en el Animator

    private Animator anim;
    private bool animacionReproducida = false;

    void Start()
    {

        GameObject j1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject j2 = GameObject.FindGameObjectWithTag("Player2");

        if (j1 != null) jugador1 = j1.transform;
        if (j2 != null) jugador2 = j2.transform;
        
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
        jugador = ObtenerJugadorObjetivo();
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
    private Transform ObtenerJugadorObjetivo()
    {
        if (jugador1 == null && jugador2 == null)
            return null;

        if (jugador1 != null && jugador2 == null)
            return jugador1;

        if (jugador2 != null && jugador1 == null)
            return jugador2;

        float dist1 = Vector2.Distance(transform.position, jugador1.position);
        float dist2 = Vector2.Distance(transform.position, jugador2.position);

        return dist1 < dist2 ? jugador1 : jugador2;
    }
}
