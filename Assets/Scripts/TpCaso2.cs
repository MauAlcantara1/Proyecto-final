using UnityEngine;
using System.Collections;

public class TpCaso2 : MonoBehaviour
{
    public Vector3 destino;

    public bool bloquearCamaraAlEntrar = true;
    public bool liberarCamaraAlLlegar = true;

    public Fade fadeOut;
    public float retraso = 1f;

    private CamaraControllerTipo2 camara;

    private bool yaTeletransportando = false;

    private void Start()
    {
        camara = Camera.main.GetComponent<CamaraControllerTipo2>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player1") || other.CompareTag("Player2")) && !yaTeletransportando)
        {
            yaTeletransportando = true;
            StartCoroutine(TeletransportarAmbos());
        }
    }

    private IEnumerator TeletransportarAmbos()
    {
        if (bloquearCamaraAlEntrar)
            camara.BloquearEnPosicionActual();

        camara.CongelarCamara(true);

        if (fadeOut != null)
            fadeOut.FadeOut();

        yield return new WaitForSeconds(retraso);

        GameObject p1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject p2 = GameObject.FindGameObjectWithTag("Player2");

        if (p1 != null) p1.transform.position = destino;
        if (p2 != null) p2.transform.position = destino;

        yield return new WaitForSeconds(0.1f);

        camara.CongelarCamara(false);

        if (liberarCamaraAlLlegar)
            camara.QuitarLimites();

        yaTeletransportando = false;
    }
}
