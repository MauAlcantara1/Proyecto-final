using UnityEngine;

public class animacionesPiernasJugador : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    void Update()
    {
        float velocidadX = rb.linearVelocity.x;
        float velocidadAbs = Mathf.Abs(velocidadX);

        animator.SetFloat("movx", velocidadAbs);

        
        }
}
