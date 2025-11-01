using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class NavInicio : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Menu");
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SalidaJuego();
        }
    }

    public void SalidaJuego()
    {
        Application.Quit();
        Debug.Log("Salir juego");
    }
    
}
