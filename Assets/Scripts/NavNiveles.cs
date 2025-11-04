using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavNiveles : MonoBehaviour
{
    [SerializeField]
    private Image[] niveles;

    [SerializeField]
    public TextMeshProUGUI textoNivel;

    [SerializeField]
    public TextMeshProUGUI textoNivelSombra;

    [SerializeField]
    private Sprite flechaRoja;

    [SerializeField]
    private Sprite flechaAmarilla;

    private int nivelActual = 0;
    private bool confirmado = false;

    void Start()
    {
        ActualizarSprites();
    }

    void Update()
    {
        if (confirmado)
            return;
        var kb = Keyboard.current;

        if (kb.escapeKey.wasPressedThisFrame)
            Regresar();

        if (kb.leftArrowKey.wasPressedThisFrame || kb.aKey.wasPressedThisFrame)
            AnteriorNivel();

        if (kb.rightArrowKey.wasPressedThisFrame || kb.dKey.wasPressedThisFrame)
            SiguienteNivel();

        if (kb.spaceKey.wasPressedThisFrame)
        {
            ConfirmarNivel();
        }
    }

    public void SiguienteNivel()
    {
        if (nivelActual < niveles.Length - 1)
        {
            nivelActual++;
            ActualizarSprites();
        }
    }

    public void AnteriorNivel()
    {
        if (nivelActual > 0)
        {
            nivelActual--;
            ActualizarSprites();
        }
    }

    private void ActualizarSprites()
    {
        for (int i = 0; i < niveles.Length; i++)
        {
            if (i == nivelActual)
                niveles[i].sprite = flechaAmarilla;
            else
                niveles[i].sprite = flechaRoja;
        }
        if (textoNivel != null)
        {
            textoNivel.text = "" + (nivelActual + 1);
            textoNivelSombra.text = "" + (nivelActual + 1);
        }
    }

    private void ConfirmarNivel()
    {
        confirmado = true;
        Debug.Log("Nivel confirmado: " + (nivelActual + 1));

        string nombreEscena = "Nivel" + (nivelActual + 1);
        SceneManager.LoadScene(nombreEscena);
    }

     public void Regresar()
    {
        SceneManager.LoadScene("Menu");
    }
}
