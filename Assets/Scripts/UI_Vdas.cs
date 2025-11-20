using UnityEngine;
using TMPro;  // si usas TextMeshPro

public class UI_Vidas : MonoBehaviour
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
        txtVidas.text = "X " + VidasPlayer.vidasJugador1;
        txtVidasSombra.text = "X " + VidasPlayer.vidasJugador1;

    }
}
