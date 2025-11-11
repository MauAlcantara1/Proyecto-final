using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoPers : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public Transform controladorDisparo;
    private Rigidbody2D rb;
    private bool isGrounded = true;

    public animacionesPiernasJugador animPiernas;
    public animacionesTorsoJugador animTorso;

    private float tiempoDisparo = 0f;
    private float duracionDisparo = 0.2f;
    private float duracionArriba = 0.2f;
    private float tiempoArriba = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (animTorso != null && animTorso.muerto)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }


        float move = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            move = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            move = 1f;

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        if (rb.linearVelocity.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (rb.linearVelocity.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);

        float velocidadAbs = Mathf.Abs(rb.linearVelocity.x);
        animPiernas?.ActualizarMovimiento(velocidadAbs);
        animTorso?.ActualizarMovimiento(velocidadAbs);

        if (Keyboard.current.wKey.wasPressedThisFrame)
            tiempoArriba = duracionArriba;

        if (Keyboard.current.wKey.isPressed)
            tiempoArriba = duracionArriba;

        if (Keyboard.current.jKey.wasPressedThisFrame)
            tiempoDisparo = duracionDisparo;

        if (Keyboard.current.jKey.isPressed)
            tiempoDisparo = duracionDisparo;

        if (tiempoDisparo > 0)
            tiempoDisparo -= Time.deltaTime;

        if (tiempoArriba > 0)
            tiempoArriba -= Time.deltaTime;

        bool Arriba = tiempoArriba > 0;
        animTorso?.ActualizarPosicion(Arriba);

        bool Disparo = tiempoDisparo > 0;
        animTorso?.ActualizarDisparo(Disparo);

        // Dirección del disparo
        if (tiempoArriba > 0)
        {
            controladorDisparo.right = Vector2.up;
        }
        else
        {
            controladorDisparo.right = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Daño"))
        {
            animTorso.Morir();
        }
    }
    
}
