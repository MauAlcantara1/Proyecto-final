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
        if (rb == null)
            Debug.LogError("No se encontró Rigidbody2D en el personaje!");
    }

    void Update()
    {
        float move = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            move = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            move = 1f;

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Debug.Log("Saltó");
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

        // direccion disparo
        if (tiempoArriba > 0) // Arriba
        {
            controladorDisparo.right = Vector2.up;
        }
        else
        {
            if (transform.localScale.x > 0) // derecha
                controladorDisparo.right = Vector2.right;
            else // izquierda
                controladorDisparo.right = Vector2.left;
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

}
