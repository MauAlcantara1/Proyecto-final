using UnityEngine;

public class CamaraControllerTipo2 : MonoBehaviour
{
    public Transform objetivo;

    public float offsetX = 0f;
    public float offsetY = 0f;

  
    public float tiempoSuavizado = 0.3f;

    private Vector3 velocidadSuavizado = Vector3.zero;
    private float posicionZ;
    private bool congelada = false;

    public bool usarLimites = false;
    public float limiteIzquierdo;
    public float limiteDerecho;
    public bool usarLimitesY = false;
    public float limiteInferior;
    public float limiteSuperior;

    // Control de transiciones
    private bool suavizarX = false;
    private bool suavizarY = false;

    private void Start()
    {
        posicionZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (objetivo == null || congelada) return;

        float nuevaX = objetivo.position.x + offsetX;
        float nuevaY = objetivo.position.y + offsetY;

        // Aplicar límites activos
        if (usarLimites)
            nuevaX = Mathf.Clamp(nuevaX, limiteIzquierdo, limiteDerecho);

        if (usarLimitesY)
            nuevaY = Mathf.Clamp(nuevaY, limiteInferior, limiteSuperior);

        Vector3 posActual = transform.position;

        if (suavizarX)
            posActual.x = Mathf.Lerp(posActual.x, nuevaX, Time.deltaTime / tiempoSuavizado);
        else
            posActual.x = nuevaX;

        if (suavizarY)
            posActual.y = Mathf.Lerp(posActual.y, nuevaY, Time.deltaTime / tiempoSuavizado);
        else
            posActual.y = nuevaY;

        posActual.z = posicionZ;

        transform.position = posActual;

        if (Mathf.Abs(transform.position.x - nuevaX) < 0.02f) suavizarX = false;
        if (Mathf.Abs(transform.position.y - nuevaY) < 0.02f) suavizarY = false;
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
        usarLimitesY = false;
    }

    public void EstablecerLimites(float izq, float der)
    {
        limiteIzquierdo = izq;
        limiteDerecho = der;
        usarLimites = true;
        suavizarX = true; 
    }

    public void EstablecerLimitesY(float abajo, float arriba)
    {
        limiteInferior = abajo;
        limiteSuperior = arriba;
        usarLimitesY = true;
        suavizarY = true; 
    }

    public void CongelarCamara(bool estado)
    {
        congelada = estado;
    }
}
