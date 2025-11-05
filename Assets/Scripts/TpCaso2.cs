using UnityEngine;
using System.Collections;

public class TpCaso2 : MonoBehaviour
{
    [Header("Destino del teletransporte")]
    public Vector3 destino;

    [Header("Opciones de cámara")]
    public bool bloquearCamaraAlEntrar = true;
    public bool liberarCamaraAlLlegar = true;

    [Header("Efectos visuales")]
    public Fade fadeOut;
    public float retraso = 1f;

    private CamaraControllerTipo2 camara;

    private void Start()
    {
        camara = Camera.main.GetComponent<CamaraControllerTipo2>();
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
        if (bloquearCamaraAlEntrar)
            camara.BloquearEnPosicionActual();

        camara.CongelarCamara(true);
        if (fadeOut != null)
            fadeOut.FadeOut();

        yield return new WaitForSeconds(retraso);

        jugador.position = destino;

        yield return new WaitForSeconds(0.1f);

        camara.CongelarCamara(false);

        if (liberarCamaraAlLlegar)
            camara.QuitarLimites();
    }
}
