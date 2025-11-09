using UnityEngine;
using TMPro;

public class Objetos_P : MonoBehaviour
{
    public TextMeshProUGUI textoPuntuacion;
    public TextMeshProUGUI textoPuntuacion2;

    [SerializeField] private AudioSource audioSource;       // <-- nuevo
    [SerializeField] private AudioClip sfxObjetoRecogido;   // <-- nuevo

    void Start()
    {
        if (textoPuntuacion != null)
            textoPuntuacion.text = "Puntos: " + VidasPlayer.puntuacion;
        if (textoPuntuacion2 != null)
            textoPuntuacion2.text = "Puntos: " + VidasPlayer.puntuacion;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entró al trigger con: " + collision.name);

        if (collision.CompareTag("ObjetoPuntos")) SumarPuntos(10, collision);
        else if (collision.CompareTag("Rubi")) SumarPuntos(200, collision);
        else if (collision.CompareTag("Diamante")) SumarPuntos(500, collision);
        else if (collision.CompareTag("Oro")) SumarPuntos(100, collision);
        else if (collision.CompareTag("Rata")) SumarPuntos(10, collision);
        else if (collision.CompareTag("Medalla")) SumarPuntos(50, collision);
        else if (collision.CompareTag("Atila")) SumarPuntos(25, collision);
        else if (collision.CompareTag("Carta")) SumarPuntos(25, collision);
        else if (collision.CompareTag("Osito")) SumarPuntos(30, collision);
        else if (collision.CompareTag("KK")) SumarPuntos(1, collision);
    }

    private void SumarPuntos(int puntos, Collider2D collision)
    {
        VidasPlayer.puntuacion += puntos;

        // reproduce sonido
        if (audioSource != null && sfxObjetoRecogido != null)
        {
            audioSource.PlayOneShot(sfxObjetoRecogido);
        }

        if (textoPuntuacion != null)
            textoPuntuacion.text = "Puntos: " + VidasPlayer.puntuacion;
        if (textoPuntuacion2 != null)
            textoPuntuacion2.text = "Puntos: " + VidasPlayer.puntuacion;

        Destroy(collision.gameObject);
    }
}

