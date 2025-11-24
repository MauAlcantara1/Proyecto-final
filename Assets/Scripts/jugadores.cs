using UnityEngine;
using UnityEngine.UI;

public class ControlModoJuegoUI : MonoBehaviour
{
    public Image botonUnJugador;
    public Image botonDosJugadores;

    private void Start()
    {
        ActualizarOpacidad();
    }

    public void SeleccionarUnJugador()
    {
        VidasPlayer.dosJugadores = false;
        ActualizarOpacidad();
    }

    public void SeleccionarDosJugadores()
    {
        VidasPlayer.dosJugadores = true;
        ActualizarOpacidad();
    }

    private void ActualizarOpacidad()
    {
        Color c1 = botonUnJugador.color;
        Color c2 = botonDosJugadores.color;

        if (VidasPlayer.dosJugadores)
        {
            c1.a = 1f; 
            c2.a = 1f; 
        }
        else
        {
            c1.a = 1f;   
            c2.a = 0.1f;
        }

        botonUnJugador.color = c1;
        botonDosJugadores.color = c2;
    }
}
