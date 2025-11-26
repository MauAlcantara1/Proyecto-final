using UnityEngine;

public class VidasPlayer : MonoBehaviour
{
    public static int vidasJugador1 = 0;
    public static int vidasJugador2 = 0;

    public static int puntuacion1 = 0;
    public static int puntuacion2 = 0;

    public static bool dosJugadores = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void UnJugador()
    {
        dosJugadores = false;
        AsignarVidas();
    }

    private void DosJugadores()
    {
        dosJugadores= true;
        AsignarVidas();
    }

    public static void AsignarVidas()
    {
        if(dosJugadores==true)
        {
            vidasJugador1 = 10;
            vidasJugador2= 10;
            puntuacion1=0;
            puntuacion2=0;
        }
        if(dosJugadores==false)
        {
            vidasJugador1=10;
            vidasJugador2= 0;
            puntuacion1=0;
            puntuacion2=0;
        }

    }
}
