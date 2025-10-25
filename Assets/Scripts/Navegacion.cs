using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Navegacion : MonoBehaviour
{
    public string escenaSig = "";
    public string escenaAnt = "";


     void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CambioEscena();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(escenaAnt == "Exit"){
                SalidaJuego();
            }else{
                RegresoEscena();
            }
        }
    }

    public void CambioEscena()
    {
        SceneManager.LoadScene(escenaSig);
    }

     public void RegresoEscena()
    {
        SceneManager.LoadScene(escenaAnt);
    }

    public void SalidaJuego(){
        Application.Quit();
        Debug.Log("Salir juego");
    }

}
