using UnityEngine;

public class CamaraController : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform objetivo;

    [Header("Configuración de cámara")]
    public float velocidadCamara = 2f;  // Qué tan rápido sigue al jugador
    public float offsetX = 2f;           // Desplazamiento horizontal de la cámara respecto al jugador

    private float posicionY;  // Altura fija de la cámara
    private float posicionZ;  // Profundidad fija de la cámara

    private void Start()
    {
        // Guardamos la posición Y y Z iniciales para mantenerlas fijas
        posicionY = transform.position.y;
        posicionZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (objetivo == null) return;

        // Calcula la nueva posición X con suavizado
        float nuevaX = Mathf.Lerp(transform.position.x, objetivo.position.x + offsetX, velocidadCamara);

        // Mantiene la cámara fija en Y y Z
        transform.position = new Vector3(nuevaX, posicionY, posicionZ);
    }
}
