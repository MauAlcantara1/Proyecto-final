using UnityEngine;

public class ZonaCamara3 : MonoBehaviour
{
    public float limiteSuperior;
    public float limiteInferior;

    private CamaraControllerTipo2 camara;

    private void Start()
    {
        camara = Camera.main.GetComponent<CamaraControllerTipo2>();
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            camara.EstablecerLimitesY(limiteInferior, limiteSuperior);
        }
    }
}
