using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;


public class NavFinal : MonoBehaviour
{
    public TextMeshProUGUI textoPuntuacion;
    public TextMeshProUGUI textoSombra;
    public float duracionConteo = 1.5f;
    public float velocidadParpadeo = 1.5f;
    

    void Start()
    {
        if (textoPuntuacion != null)
            StartCoroutine(ContarPuntuacion(VidasPlayer.puntuacion1));
    }

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

    IEnumerator ContarPuntuacion(int puntuacionFinal)
    {
        int valorActual = 0;
        float tiempo = 0f;

        while (tiempo < duracionConteo)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracionConteo);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            valorActual = Mathf.RoundToInt(puntuacionFinal * eased);

            textoPuntuacion.text = valorActual.ToString();
            if (textoSombra != null)
                textoSombra.text = textoPuntuacion.text;

            yield return null;
        }

        textoPuntuacion.text = puntuacionFinal.ToString();
        if (textoSombra != null)
            textoSombra.text = textoPuntuacion.text;

        yield return StartCoroutine(PulseText(textoPuntuacion));

        StartCoroutine(ParpadearTexto(textoPuntuacion));
    }

    IEnumerator PulseText(TextMeshProUGUI texto)
    {
        Vector3 original = texto.transform.localScale;
        Vector3 grande = original * 1.2f;
        float dur = 0.15f;
        float t = 0;

        while (t < dur)
        {
            t += Time.deltaTime;
            texto.transform.localScale = Vector3.Lerp(original, grande, t / dur);
            yield return null;
        }

        t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            texto.transform.localScale = Vector3.Lerp(grande, original, t / dur);
            yield return null;
        }

        texto.transform.localScale = original;
    }

    IEnumerator ParpadearTexto(TextMeshProUGUI texto)
    {
        Color colorOriginal = texto.color;
        Color transparente = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0.3f);

        while (true)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * velocidadParpadeo;
                texto.color = Color.Lerp(colorOriginal, transparente, t);
                yield return null;
            }

            t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * velocidadParpadeo;
                texto.color = Color.Lerp(transparente, colorOriginal, t);
                yield return null;
            }
        }
    }

    
}
