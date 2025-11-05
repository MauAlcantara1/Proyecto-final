using UnityEngine;

public class CamaraControllerTipo2 : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform objetivo;

    [Header("Configuración de cámara")]
    public float velocidadCamara = 5f;
    public float offsetX = 0f;
    public float offsetY = 0f;
    [Tooltip("Distancia mínima antes de mover la cámara")]
    public float toleranciaMovimiento = 0.05f;

    [Header("Límites de cámara (opcional)")]
    public bool usarLimites = false;
    public float limiteIzquierdo;
    public float limiteDerecho;
    public bool usarLimitesY = false;
    public float limiteInferior;
    public float limiteSuperior;

    private float posicionZ;
    private bool congelada = false;

    private void Start()
    {
        posicionZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (objetivo == null || congelada) return;

        // Calcular destino en X e Y
        float destinoX = objetivo.position.x + offsetX;
        float destinoY = objetivo.position.y + offsetY;

        // Solo mover si hay diferencia significativa
        float diferenciaX = Mathf.Abs(destinoX - transform.position.x);
        float diferenciaY = Mathf.Abs(destinoY - transform.position.y);

        float nuevaX = transform.position.x;
        float nuevaY = transform.position.y;

        if (diferenciaX > toleranciaMovimiento)
        {
            nuevaX = Mathf.MoveTowards(
                transform.position.x,
                destinoX,
                velocidadCamara * Time.deltaTime
            );
        }

        if (diferenciaY > toleranciaMovimiento)
        {
            nuevaY = Mathf.MoveTowards(
                transform.position.y,
                destinoY,
                velocidadCamara * Time.deltaTime
            );
        }

        // Aplicar límites
        if (usarLimites)
            nuevaX = Mathf.Clamp(nuevaX, limiteIzquierdo, limiteDerecho);

        if (usarLimitesY)
            nuevaY = Mathf.Clamp(nuevaY, limiteInferior, limiteSuperior);

        transform.position = new Vector3(nuevaX, nuevaY, posicionZ);
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
    }

    public void EstablecerLimitesY(float abajo, float arriba)
    {
        limiteInferior = abajo;
        limiteSuperior = arriba;
        usarLimitesY = true;
    }

    public void CongelarCamara(bool estado)
    {
        congelada = estado;
    }
}
