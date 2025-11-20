using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class NavSalir : MonoBehaviour
{
    void Update(){
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            VidasPlayer.vidasJugador1 = 10;
            VidasPlayer.vidasJugador2 = 10;
            VidasPlayer.puntuacion1 = 0;
            VidasPlayer.puntuacion2 = 0;
            SceneManager.LoadScene("Menu");
        }
    }
}
