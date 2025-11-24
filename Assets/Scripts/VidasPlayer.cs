using UnityEngine;

public class VidasPlayer : MonoBehaviour
{
    public static int vidasJugador1 = 10;
    public static int vidasJugador2 = 10;

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
    }

    private void DosJugadores()
    {
        dosJugadores= true;
    }
}
