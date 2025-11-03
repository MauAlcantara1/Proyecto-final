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
        imagen.fillOrigin = 1;
        imagen.fillAmount = 0;
    }


    public IEnumerator Transicion(Vector3 nuevaPos, Transform jugador)
    {
        imagen.fillOrigin = 1; 
        imagen.fillMethod = Image.FillMethod.Horizontal;

        for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
        {
            imagen.fillAmount = t;
            yield return null;
        }

        imagen.fillAmount = 1f;

        jugador.position = nuevaPos;

        yield return new WaitForSeconds(0.2f);
        for (float t = 1f; t > 0f; t -= Time.deltaTime * 2f)
        {
            imagen.fillAmount = t;
            yield return null;
        }

        imagen.fillAmount = 0f;
    }
}
