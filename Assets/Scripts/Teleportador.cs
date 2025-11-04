using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class Teleportador : MonoBehaviour
{
    public Vector3 destino;
    public Fade fadeOut;
    public float retraso = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeletransportarConRetraso(other.transform));
        }
    }

    private IEnumerator TeletransportarConRetraso(Transform jugador)
    {
        fadeOut.FadeOut();
        yield return new WaitForSeconds(retraso);
        jugador.position = destino;
    }
}
