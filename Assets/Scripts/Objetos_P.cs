using UnityEngine;
using UnityEngine.UI;

public class Objetos_P : MonoBehaviour
{
    public Text textoPuntuacion;

    void Start()
    {
        // Puntuación inicial
        if (textoPuntuacion != null)
            textoPuntuacion.text = "Puntos: " + VidasPlayer.puntuacion;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entró al trigger con: " + collision.name);


        if (collision.CompareTag("ObjetoPuntos"))
        {
            SumarPuntos(10, collision);
        }

        else if (collision.CompareTag("Rubi"))
        {
            SumarPuntos(20, collision);
        }

        else if (collision.CompareTag("Diamante"))
        {
            SumarPuntos(50, collision);
        }

        else if (collision.CompareTag("Oro"))
        {
            SumarPuntos(15, collision);
        }

        else if (collision.CompareTag("Rata"))
        {
            SumarPuntos(70, collision);
        }

        else if (collision.CompareTag("Medalla"))
        {
            SumarPuntos(100, collision);
        }

        else if (collision.CompareTag("Atila"))
        {
            SumarPuntos(120, collision);
        }

        else if (collision.CompareTag("Carta"))
        {
            SumarPuntos(55, collision);
        }

        else if (collision.CompareTag("Osito"))
        {
            SumarPuntos(150, collision);
        }

        else if (collision.CompareTag("KK"))
        {
            SumarPuntos(500, collision);
        }
    }

    private void SumarPuntos(int puntos, Collider2D collision)
    {
        VidasPlayer.puntuacion += puntos;
        Debug.Log($"¡Objeto recogido ({collision.tag})! Puntuación: {VidasPlayer.puntuacion}");

        if (textoPuntuacion != null)
            textoPuntuacion.text = "Puntos: " + VidasPlayer.puntuacion;

        Destroy(collision.gameObject);
    }
}
