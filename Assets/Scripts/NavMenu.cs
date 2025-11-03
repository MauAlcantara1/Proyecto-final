using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavMenu : MonoBehaviour
{
    [SerializeField] private RectTransform flecha;
    [SerializeField] private GameObject panelSonido;
    [SerializeField] private GameObject panelMenu;

    private int opcionActual = 1;
    private bool confirmado = false;

    void Update()
    {
        if (confirmado) return;

        var kb = Keyboard.current;

        if (kb.upArrowKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame )
            CambiarOpcion(1);
        if (kb.downArrowKey.wasPressedThisFrame || kb.sKey.wasPressedThisFrame)
            CambiarOpcion(2);

        if (kb.spaceKey.wasPressedThisFrame || kb.enterKey.wasPressedThisFrame)
            ConfirmarSeleccion();

        if (kb.escapeKey.wasPressedThisFrame)
            Regresar();
    }

    public void ClickNiveles()
    {
        if (opcionActual == 1)
            ConfirmarSeleccion();
        else
            CambiarOpcion(1);
    }

    public void ClickSonido()
    {
        if (opcionActual == 2)
            ConfirmarSeleccion();
        else
            CambiarOpcion(2);
    }

    private void ConfirmarSeleccion()
    {
        confirmado = true;
        if (opcionActual == 1)
            Niveles();
        else
            SonidoOpciones();
    }

    public void Niveles()
    {
        SceneManager.LoadScene("Niveles");
    }
     public void Creditos()
    {
        SceneManager.LoadScene("Info");
    }

    public void SonidoOpciones()
    {
        panelSonido.SetActive(true);
        panelMenu.SetActive(false);
        confirmado = false;
    }

    public void SalirSonido()
    {
        panelSonido.SetActive(false);
        panelMenu.SetActive(true);
        confirmado = false;
    }

    public void Regresar()
    {
        SceneManager.LoadScene("Inicio");
    }

    private void CambiarOpcion(int nuevaOpcion)
    {
        opcionActual = nuevaOpcion;
        ActualizarFlecha();
    }

    private void ActualizarFlecha()
    {
        if (opcionActual == 1)
            flecha.anchoredPosition = new Vector2(flecha.anchoredPosition.x, 45);
        else if (opcionActual == 2)
            flecha.anchoredPosition = new Vector2(flecha.anchoredPosition.x, -100);
    }
}
