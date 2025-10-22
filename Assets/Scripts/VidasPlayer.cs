using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VidasPlayer : MonoBehaviour
{
    public static int vida;
    public static int puntuacion = 0; // 👈 Puntuación global

    public Image barraVida;
    private bool haMuerto;

    void Start()
    {
        haMuerto = false;
        vida = 5;
        ActualizarBarraVida();
    }

    public void TomarDaño(int daño)
    {
        // Si quieres animaciones de daño según dirección, puedes comentar estas líneas
        // GetComponent<Animator>().SetTrigger("daño");

        vida -= daño;
        ActualizarBarraVida();

        if (vida <= 0 && !haMuerto)
        {
            haMuerto = true;
            GetComponent<Animator>().SetBool("muerte", true);
            StartCoroutine(EjecutarMuerte());
        }
    }

    private void ActualizarBarraVida()
    {
        if (barraVida != null)
        {
            barraVida.fillAmount = (float)vida / 5f;
        }
    }

    IEnumerator EjecutarMuerte()
    {
        yield return new WaitForSeconds(2.1f);
        Debug.Log("Puntuación final: " + puntuacion);
        StartCoroutine(RegresaMenu());
    }

    IEnumerator RegresaMenu()
    {
        yield return new WaitForSeconds(2.7f);
        SceneManager.LoadScene("Menu");
        Destroy(gameObject);
    }
}
