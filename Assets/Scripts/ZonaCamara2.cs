using UnityEngine;

public class ZonaCamara2 : MonoBehaviour
{
    public float limiteDerecho;
    public float limiteIzquierdo;

    private CamaraControllerTipo2 camara;

    private void Start()
    {
        camara = Camera.main.GetComponent<CamaraControllerTipo2>();
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            camara.EstablecerLimites(limiteIzquierdo, limiteDerecho);
        }
    }
}
