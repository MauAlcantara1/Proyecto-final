using UnityEngine;

public class Balatanque : MonoBehaviour
{
    [Header("Propiedades")]
    public float velocidad = 10f;
    public float tiempoVida = 3f;
    public int daño = 1;

    void Start()
    {
        // Destruye la bala después de un tiempo para evitar acumular basura
        Destroy(gameObject, tiempoVida);
    }
}