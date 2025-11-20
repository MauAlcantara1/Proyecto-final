using UnityEngine;
using TMPro; 

public class UI_Vidas2 : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TMP_Text txtVidas;
    [SerializeField] private TMP_Text txtVidasSombra;

    private void Start()
    {
        ActualizarVidas();
    }

    private void Update()
    {
        ActualizarVidas();
    }

    private void ActualizarVidas()
    {
        txtVidas.text = "X " + VidasPlayer.vidasJugador2;
        txtVidasSombra.text = "X " + VidasPlayer.vidasJugador2;

    }
}
