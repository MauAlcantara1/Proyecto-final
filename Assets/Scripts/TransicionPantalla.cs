using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransicionPantalla : MonoBehaviour
{
    private Image imagen;

    void Awake()
    {
        imagen = GetComponent<Image>();
        imagen.type = Image.Type.Filled;      // <-- esto fuerza que sea Filled
        imagen.fillMethod = Image.FillMethod.Horizontal;
        imagen.fillOrigin = 1; // derecha
        imagen.fillAmount = 0;
    }


    public IEnumerator Transicion(Vector3 nuevaPos, Transform jugador)
    {
        // --- Fase 1: aparece de derecha a izquierda ---
        imagen.fillOrigin = 1; // derecha
        imagen.fillMethod = Image.FillMethod.Horizontal;

        for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
        {
            imagen.fillAmount = t;
            yield return null;
        }

        imagen.fillAmount = 1f;

        // --- Teletransporta jugador ---
        jugador.position = nuevaPos;

        // Espera un pequeño momento para que se note la transición
        yield return new WaitForSeconds(0.2f);

        // --- Fase 2: desaparece ---
        for (float t = 1f; t > 0f; t -= Time.deltaTime * 2f)
        {
            imagen.fillAmount = t;
            yield return null;
        }

        imagen.fillAmount = 0f;
    }
}
