using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class NavSalir : MonoBehaviour
{
    void Update(){
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            VidasPlayer.vidas = 7;
            VidasPlayer.puntuacion = 0;
            SceneManager.LoadScene("Menu");
        }
    }
}
