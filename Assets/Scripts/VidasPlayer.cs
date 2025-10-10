using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VidasPlayer : MonoBehaviour
{
    public static int vida;
    public Image[] vidas;
    private bool haMuerto;
    public GameObject gameOver;

    void Start()
    {
        haMuerto = false;
        vida = 5;
        DibujarVidas(vida);
        gameOver.SetActive(false);
    }

    void Update()
    {
        
    }

    public void TomarDaño(int daño)
    {
        if (MovimientoPlayer.dirIdle == 1)
        {
            GetComponent<Animator>().SetTrigger("daño");
        }
        else if (MovimientoPlayer.dirIdle == -1)
        {
            GetComponent<Animator>().SetTrigger("dañoIzq");
        }

        vida -= daño;
        DibujarVidas(vida);

        if (vida <= 0 && !haMuerto)
        {
            haMuerto = true;
            GetComponent<Animator>().SetBool("muerte", true);
            StartCoroutine(EjecutarMuerte());
        }
    }

    public void DibujarVidas(int n)
    {
        for (int i = 0; i < vidas.Length; i++)
        {
            vidas[i].enabled = false;
        }
        for (int i = 0; i < n; i++)
        {
            vidas[i].enabled = true;
        }
    }

    IEnumerator EjecutarMuerte()
    {
        yield return new WaitForSeconds(2.1f);
        gameOver.SetActive(true);
        StartCoroutine(RegresaMenu());
    }

    IEnumerator RegresaMenu()
    {
        yield return new WaitForSeconds(2.7f);
        SceneManager.LoadScene("Menu");
        Destroy(gameObject);
    }
}
