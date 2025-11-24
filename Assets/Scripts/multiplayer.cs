using UnityEngine;

public class multiplayer : MonoBehaviour
{
    [SerializeField] private GameObject jugadorDos;
    [SerializeField] private GameObject jugadorDosVidas;
    [SerializeField] private GameObject jugadorDosPuntuacion;



    void Start()
    {
        if (VidasPlayer.dosJugadores == true)
        {
            jugadorDos.SetActive(true);
            jugadorDosPuntuacion.SetActive(true);
            jugadorDosVidas.SetActive(true);
        }
        else
        {
            jugadorDos.SetActive(false);
            jugadorDosPuntuacion.SetActive(false);
            jugadorDosVidas.SetActive(false);
        }
    }
}
