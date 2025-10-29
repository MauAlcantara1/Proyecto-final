using UnityEngine;
using System.Collections;
public class animacionesTorsoJugador : MonoBehaviour
{
    private Animator animator;

    public SpriteRenderer piernas;

    void Start()
    {
        animator = GetComponent<Animator>();
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

    public void MostrarSprite()
    {
        if (piernas != null)
            piernas.enabled = true;
    }
}
