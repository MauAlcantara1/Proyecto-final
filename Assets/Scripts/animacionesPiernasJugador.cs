using UnityEngine;

public class animacionesPiernasJugador : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ActualizarMovimiento(float movx)
    {
        animator.SetFloat("movx", movx);
    }
}
