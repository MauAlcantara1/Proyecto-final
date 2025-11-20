using UnityEngine;
using TMPro;

public class Objetos_P : MonoBehaviour
{
    public TextMeshProUGUI textoPuntuacion1;
    public TextMeshProUGUI textoPuntuacion2;

    public TextMeshProUGUI textoPuntuacion3;
    public TextMeshProUGUI textoPuntuacion4;

    private void Start()
    {
        ActualizarUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entró al trigger con: " + collision.name);

        int puntos = ObtenerPuntosPorTag(collision.tag);
        if (puntos == 0) return;

        if (gameObject.CompareTag("Player1"))
        {
            SumarPuntosJugador1(puntos);
        }
        else if (gameObject.CompareTag("Player2"))
        {
            SumarPuntosJugador2(puntos);
        }

        Destroy(collision.gameObject);
    }


    int ObtenerPuntosPorTag(string tag)
    {
        switch (tag)
        {
            case "ObjetoPuntos": return 10;
            case "Rubi": return 200;
            case "Diamante": return 500;
            case "Oro": return 100;
            case "Rata": return 10;
            case "Medalla": return 50;
            case "Atila": return 25;
            case "Carta": return 25;
            case "Osito": return 30;
            case "KK": return 1;
        }
        return 0;
    }

    void SumarPuntosJugador1(int puntos)
    {
        VidasPlayer.puntuacion1 += puntos;
        ActualizarUI();
    }

    void SumarPuntosJugador2(int puntos)
    {
        VidasPlayer.puntuacion2 += puntos;
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (textoPuntuacion1 != null)
            textoPuntuacion1.text = "Puntos: " + VidasPlayer.puntuacion1;

        if (textoPuntuacion2 != null)
            textoPuntuacion2.text = "Puntos: " + VidasPlayer.puntuacion1;

        if (textoPuntuacion3 != null)
            textoPuntuacion3.text = "Puntos: " + VidasPlayer.puntuacion2;

        if (textoPuntuacion4 != null)
            textoPuntuacion4.text = "Puntos: " + VidasPlayer.puntuacion2;
    }
}

