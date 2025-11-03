using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NavInfo : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Regreso();
        }
    }

    public void Regreso()
    {
        SceneManager.LoadScene("Menu");
    }
}
