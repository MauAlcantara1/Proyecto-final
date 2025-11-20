using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarNivel : MonoBehaviour
{
    public string nombreEscena;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            VidasPlayer.vidasJugador1 += 3;
            VidasPlayer.vidasJugador2 += 3;

            if (!string.IsNullOrEmpty(nombreEscena))
            {
                SceneManager.LoadScene(nombreEscena);
            }
        }
    }
}
