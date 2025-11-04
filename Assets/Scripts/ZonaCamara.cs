using UnityEngine;

public class ZonaCamara : MonoBehaviour
{
    public float limiteIzquierdo;
    public float limiteDerecho;
    private CamaraController camara;

    private void Start()
    {
        camara = Camera.main.GetComponent<CamaraController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            camara.EstablecerLimites(limiteIzquierdo, limiteDerecho);
        }
    }
    
}
