using UnityEngine;

public class Tanque : MonoBehaviour
{
    [Header("Movimiento de Patrullaje")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float distanciaPatrulla = 4f;

    [Header("Detección del Jugador")]
    [SerializeField] private float radioDeteccion = 10f;
    private Transform jugador;

    private Vector3 puntoInicial;
    private Vector3 puntoFinal;
    private bool yendoAlFinal = true;

    private bool persiguiendo = false;

    private void Start()
    {
        puntoInicial = transform.position;
        puntoFinal = puntoInicial + transform.forward * distanciaPatrulla;

        // Buscar al jugador por Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            jugador = playerObj.transform;
            Debug.Log("Jugador encontrado.");
        }
        else
        {
            Debug.LogWarning("No se encontró objeto con tag 'Player'.");
        }
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, puntoFinal, Color.green);

        if (jugador != null)
        {
            DetectarJugador();
        }

        if (persiguiendo)
        {
            PerseguirJugador();
        }
        else
        {
            Patrullar();
        }
    }

    private void DetectarJugador()
    {
        float distancia = Vector3.Distance(transform.position, jugador.position);

        // Si está en rango, activa persecución
        if (distancia < radioDeteccion)
        {
            persiguiendo = true;
        }
        else
        {
            persiguiendo = false;
        }
    }

    private void PerseguirJugador()
    {
        // Mover hacia el jugador
        transform.position = Vector3.MoveTowards(
            transform.position,
            jugador.position,
            velocidad * Time.deltaTime
        );

        Debug.Log("Persiguiendo al jugador...");
    }

    private void Patrullar()
    {
        Vector3 objetivo = yendoAlFinal ? puntoFinal : puntoInicial;

        transform.position = Vector3.MoveTowards(
            transform.position,
            objetivo,
            velocidad * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, objetivo) < 0.05f)
        {
            yendoAlFinal = !yendoAlFinal;
        }
    }
}
