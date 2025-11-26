using UnityEngine;

public class CamaraController : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform objetivo;
    public Transform objetivo2;
    private Transform objetivoVivo;

    [Header("Configuración de cámara")]
    public float velocidadCamara = 5f;
    public float offsetX = 2f;
    [Tooltip("Distancia mínima antes de mover la cámara")]
    public float toleranciaMovimiento = 0.05f;

    [Header("Límites de cámara (opcional)")]
    public bool usarLimites = false;
    public float limiteIzquierdo;
    public float limiteDerecho;

    private float posicionY;
    private float posicionZ;
    private bool congelada = false;

    private void Start()
    {
        posicionY = transform.position.y;
        posicionZ = transform.position.z;
        JugadorActivo();
    }

    private void Update()
    {
        JugadorActivo(); 
    }

    private void JugadorActivo()
    {
        bool j1Vivo = (objetivo != null && objetivo.gameObject.activeInHierarchy);
        bool j2Vivo = (objetivo2 != null && objetivo2.gameObject.activeInHierarchy);

        if (j1Vivo)
            objetivoVivo = objetivo;
        else if (j2Vivo)
            objetivoVivo = objetivo2;
        else
            objetivoVivo = null; 
    }


    private void LateUpdate()
    {
        if (objetivoVivo == null || congelada) return;

        float destinoX = objetivo.position.x + offsetX;
        float diferencia = Mathf.Abs(destinoX - transform.position.x);

        if (diferencia > toleranciaMovimiento)
        {
            float nuevaX = Mathf.MoveTowards(
                transform.position.x,
                destinoX,
                velocidadCamara * Time.deltaTime
            );

            if (usarLimites)
                nuevaX = Mathf.Clamp(nuevaX, limiteIzquierdo, limiteDerecho);

            transform.position = new Vector3(nuevaX, posicionY, posicionZ);
        }
    }

    public void BloquearEnPosicionActual()
    {
        usarLimites = true;
        limiteIzquierdo = transform.position.x;
        limiteDerecho = transform.position.x;
    }

    public void QuitarLimites()
    {
        usarLimites = false;
    }

    public void EstablecerLimites(float izq, float der)
    {
        limiteIzquierdo = izq;
        limiteDerecho = der;
        usarLimites = true;
    }

    public void CongelarCamara(bool estado)
    {
        congelada = estado;
    }
}
