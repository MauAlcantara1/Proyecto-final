using UnityEngine;

public class BalatanqueBola : MonoBehaviour
{
    [Header("Propiedades")]
    public float velocidad = 10f;
    public float tiempoVida = 3f;
    public int daño = 1;

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player1") || other.collider.CompareTag("Player2"))
        {
            Destroy(gameObject);
        }
        
    }


}