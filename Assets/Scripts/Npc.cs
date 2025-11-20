using UnityEngine;

public class Npc : MonoBehaviour
{
    private Transform jugador;
    private Transform jugador1;
    private Transform jugador2;
    private bool activo = false;

    [Header("Movimiento")]
    public float dActivacion = 10f;        
    public float velHuida = 4f;            
    public float limiteFueraCamara = 15f;  // Distancia máxima antes de destruirse

    void Start()
    {
        GameObject j1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject j2 = GameObject.FindGameObjectWithTag("Player2");

        if (j1 != null) jugador1 = j1.transform;
        if (j2 != null) jugador2 = j2.transform;
    }

    void Update()
    {
        jugador = ObtenerJugadorObjetivo();
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (!activo && distancia <= dActivacion)
        {
            activo = true;
        }

        if (activo)
        {
            Huir();

            if (distancia > limiteFueraCamara)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Huir()
    {
        transform.position += Vector3.left * velHuida * Time.deltaTime;
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
