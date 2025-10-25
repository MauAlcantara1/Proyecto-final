using UnityEngine;
using TMPro;

public class TextoParpadeo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI texto; 
    [SerializeField] private float velocidadParpadeo;

    private Color colorOriginal;

    void Start()
    {
        if (texto == null)
            texto = GetComponent<TextMeshProUGUI>();

        colorOriginal = texto.color;
        StartCoroutine(Parpadear());
    }

    System.Collections.IEnumerator Parpadear()
    {
        while (true)
        {
            float alpha = (Mathf.Sin(Time.time * velocidadParpadeo) + 1f) / 2f;

            texto.color = new Color(
                colorOriginal.r,
                colorOriginal.g,
                colorOriginal.b,
                alpha
            );

            yield return null;
        }
    }
}
