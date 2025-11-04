using UnityEngine;

public class CamaraController : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform objetivo;

    [Header("Configuración de cámara")]
    public float velocidadCamara = 5f;
    public float offsetX = 2f;

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
    }

    private void LateUpdate()
    {
        if (objetivo == null) return;

        // Calcular nueva posición X
        float nuevaX = Mathf.Lerp(transform.position.x, objetivo.position.x + offsetX, Time.deltaTime * velocidadCamara);

        // Aplicar límites si están activos
        if (usarLimites)
        {
            nuevaX = Mathf.Clamp(nuevaX, limiteIzquierdo, limiteDerecho);
        }

        transform.position = new Vector3(nuevaX, posicionY, posicionZ);
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
