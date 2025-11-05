using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarNivel : MonoBehaviour
{
    public string nombreEscena;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(nombreEscena))
            {
                SceneManager.LoadScene(nombreEscena);
            }
            else
            {
                Debug.LogWarning("No se ha asignado una escena en el inspector.");
            }
        }
    }
}
