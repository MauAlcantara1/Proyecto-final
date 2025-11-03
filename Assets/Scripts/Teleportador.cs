using UnityEngine;

public class Teleportador : MonoBehaviour
{
    public Vector3 destino;
    public TransicionPantalla transicionPantalla;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(transicionPantalla.Transicion(destino, other.transform));
        }
    }
}
