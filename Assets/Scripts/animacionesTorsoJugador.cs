using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class animacionesTorsoJugador : MonoBehaviour
{
    private Animator animator;
    public GameObject panelMuerte;
    public SpriteRenderer piernas;
    public bool muerto = false;
    private bool inmune = false;
    private GolpeJugador golpeJugador;
    private AudioSource audioSource;
    [SerializeField] private AudioClip GameOver;

    private int vidas => VidasPlayer.vidas;


    void Start()
    {
        animator = GetComponent<Animator>();
        golpeJugador = GetComponentInChildren<GolpeJugador>();
        audioSource = GetComponent<AudioSource>();

        if (panelMuerte != null)
            panelMuerte.gameObject.SetActive(false);
    }

    public void ActualizarMovimiento(float movx)
    {
        animator.SetFloat("movx", movx);
    }

    public void ActualizarDisparo(bool Disparo)
    {
        animator.SetBool("Disparo", Disparo);
    }

    public void ActualizarPosicion(bool Arriba)
    {
        animator.SetBool("Arriba", Arriba);
    }

    public void ActualizarGolpe(bool Golpe)
    {
        animator.SetBool("Golpe", Golpe);
    }

    public void MostrarSprite()
    {
        if (piernas != null)
            piernas.enabled = true;
    }

    public void Morir()
    {
        if (muerto || inmune) return;

        if (piernas != null)
            piernas.enabled = false;

        muerto = true;

        animator.Play("muerte");

        if (audioSource != null)
            audioSource.Play();
    }

    public void Revivir()
    {
        if (!muerto) return;

        if (piernas != null)
            piernas.enabled = true;

        muerto = false;

        StartCoroutine(InvulnerabilidadTemporal(1f)); 
    }

    private IEnumerator InvulnerabilidadTemporal(float duracionExtra)
    {
        inmune = true;


        float duracionAnim = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duracionAnim + duracionExtra);

        inmune = false;
    }

    public void SistemaDeVidas()
    {
        VidasPlayer.vidas -= 1;


        Debug.Log("Vidas actuales: " + vidas);

        if (vidas <= 0)
        {
            VidasPlayer.vidas = 0;

            if (audioSource != null && GameOver != null)
                audioSource.PlayOneShot(GameOver);

            if (panelMuerte != null)
            {
                FindObjectOfType<musicaNivel>()?.PausarMusica();
                panelMuerte.SetActive(true);
            }
        }
    }

    private IEnumerator RetornoMenu()
    {
        Time.timeScale = 0f;   
        VidasPlayer.vidas = 7;
        VidasPlayer.puntuacion = 0;
        float tiempo = 0f;

        while (tiempo < 5f)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                break;

            tiempo += Time.unscaledDeltaTime; 
            yield return null;
        }

        Time.timeScale = 1f; 
        SceneManager.LoadScene("Inicio"); 
    }

    
}
