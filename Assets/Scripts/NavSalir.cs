using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class NavSalir : MonoBehaviour
{
    void Update(){
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            VidasPlayer.AsignarVidas();
            SceneManager.LoadScene("Menu");
        }
    }
}
