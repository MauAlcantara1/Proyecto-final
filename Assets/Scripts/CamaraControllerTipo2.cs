using UnityEngine;

public class CamaraControllerTipo2 : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform objetivo;

    [Header("Configuración de cámara")]
    public float offsetX = 0f;
    public float offsetY = 0f;

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

        // Calcular la posición deseada
        float nuevaX = objetivo.position.x + offsetX;
        float nuevaY = objetivo.position.y + offsetY;

        // Aplicar límites si están activos
        if (usarLimites)
            nuevaX = Mathf.Clamp(nuevaX, limiteIzquierdo, limiteDerecho);

        if (usarLimitesY)
            nuevaY = Mathf.Clamp(nuevaY, limiteInferior, limiteSuperior);

        // Mover instantáneamente la cámara
        transform.position = new Vector3(nuevaX, nuevaY, posicionZ);
    }

    // --- Métodos auxiliares ---
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
