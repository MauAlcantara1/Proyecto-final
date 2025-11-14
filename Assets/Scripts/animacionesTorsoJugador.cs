using UnityEngine;
using System.Collections;

public class animacionesTorsoJugador : MonoBehaviour
{
    private Animator animator;

    public SpriteRenderer piernas;
    public bool muerto = false;
    private bool inmune = false;
    private GolpeJugador golpeJugador;
    private AudioSource audioSource;


    void Start()
    {
        animator = GetComponent<Animator>();
        golpeJugador = GetComponentInChildren<GolpeJugador>();
        audioSource = GetComponent<AudioSource>();


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

    public void EjecutarGolpe()
    {
        if (golpeJugador != null)
        {
            golpeJugador.ActivarGolpe();
        }
        else
        {
            Debug.LogWarning("GolpeJugador no encontrado en hijos del torso");
        }
    }

    public void TerminarGolpe()
    {
        if (golpeJugador != null)
        {
            golpeJugador.DesactivarGolpe();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Morir();
        }
    }

}
