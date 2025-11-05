using UnityEngine;

public class Npc : MonoBehaviour
{
    private Transform jugador;
    private bool activo = false;

    [Header("Movimiento")]
    public float dActivacion = 10f;        
    public float velHuida = 4f;            
    public float limiteFueraCamara = 15f;  // Distancia máxima antes de destruirse

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (jugador == null)
            Debug.LogError("❌ No se encontró el objeto con tag 'Player'.");
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Activar cuando el jugador esté dentro del rango
        if (!activo && distancia <= dActivacion)
        {
            activo = true;
            Debug.Log($"{name} ha visto al jugador y huye hacia la izquierda!");
        }

        // Si está activo, huir hacia la izquierda
        if (activo)
        {
            Huir();

            // Si se aleja demasiado, destruirlo
            if (distancia > limiteFueraCamara)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Huir()
    {
        // Movimiento fijo hacia la izquierda (sin rotar)
        transform.position += Vector3.left * velHuida * Time.deltaTime;
    }
}
