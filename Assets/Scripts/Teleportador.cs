using System.Collections;
using UnityEngine;

public class Teleportador : MonoBehaviour
{
    [Header("Destino del teletransporte")]
    public Vector3 destino;

    [Header("Opciones de cámara")]
    public bool bloquearCamaraAlEntrar = true;
    public bool liberarCamaraAlLlegar = true;

    [Header("Efectos visuales")]
    public Fade fadeOut;
    public float retraso = 1f;

    private CamaraController camara;

    private void Start()
    {
        camara = Camera.main.GetComponent<CamaraController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeletransportarConRetraso(other.transform));
        }
    }

    private IEnumerator TeletransportarConRetraso(Transform jugador)
    {
        // --- Paso 1: congelar cámara en el lugar ---
        if (bloquearCamaraAlEntrar)
            camara.BloquearEnPosicionActual();

        // --- Paso 2: fade out ---
        camara.CongelarCamara(true);
        if (fadeOut != null)
            fadeOut.FadeOut();

        yield return new WaitForSeconds(retraso);

        // --- Paso 3: teletransportar ---
        jugador.position = destino;

        yield return new WaitForSeconds(0.1f);

        // --- Paso 4: liberar la cámara ---
        camara.CongelarCamara(false);

        if (liberarCamaraAlLlegar)
            camara.QuitarLimites();
    }
}
