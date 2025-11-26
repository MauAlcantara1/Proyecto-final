using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoPers2 : MonoBehaviour
{
    public float speed = 5f;
    public Key teclaGolpe; 
    public float jumpForce = 10f;
    public Transform controladorDisparo;
    private Rigidbody2D rb;
    private bool isGrounded = true;

    public animacionesPiernasJugador animPiernas;
    public animacionesTorsoJugador2 animTorso;

    private float tiempoDisparo = 0f;
    private float duracionDisparo = 0.2f;
    private float duracionArriba = 0.2f;
    private float tiempoArriba = 0;

    private float tiempoGolpe = 0;

    private float duracionGolpe = 0.2f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("enemigo");
        foreach (GameObject enemigo in enemigos)
        {
            IgnorarColisionesConEnemigo(enemigo);
        }
    }

    void Update()
    {
        if (animTorso != null && animTorso.muerto)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        float move = 0f;
        if (Keyboard.current.leftArrowKey.isPressed)
            move = -1f;
        if (Keyboard.current.rightArrowKey.isPressed)
            move = 1f;

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (Keyboard.current.rightShiftKey.wasPressedThisFrame && isGrounded)
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

        if (Keyboard.current[teclaGolpe].wasPressedThisFrame)         
            tiempoGolpe = duracionGolpe;
            

        if (Keyboard.current[teclaGolpe].isPressed)
            tiempoGolpe = duracionGolpe;
            
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            tiempoArriba = duracionArriba;

        if (Keyboard.current.upArrowKey.isPressed)
            tiempoArriba = duracionArriba;

        if (Keyboard.current.periodKey.wasPressedThisFrame)
            tiempoDisparo = duracionDisparo;

        if (Keyboard.current.periodKey.isPressed)
            tiempoDisparo = duracionDisparo;


        if (tiempoDisparo > 0)
            tiempoDisparo -= Time.deltaTime;

        if (tiempoArriba > 0)
            tiempoArriba -= Time.deltaTime;

        if (tiempoGolpe > 0)
            tiempoGolpe -= Time.deltaTime;

        bool Arriba = tiempoArriba > 0;
        animTorso?.ActualizarPosicion(Arriba);

        bool Disparo = tiempoDisparo > 0;
        animTorso?.ActualizarDisparo(Disparo);

        bool Golpe = tiempoGolpe > 0;
        animTorso?.ActualizarGolpe(Golpe);

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
        if (collision.CompareTag("DañoEnemigo"))
        {
            animTorso.Morir();
        }
    }

    void IgnorarColisionesConEnemigo(GameObject enemigo)
    {
        Collider2D[] colJugador = GetComponents<Collider2D>();
        Collider2D[] colEnemigo = enemigo.GetComponents<Collider2D>();

        foreach (Collider2D cj in colJugador)
        {
            foreach (Collider2D ce in colEnemigo)
            {
                if (!cj.isTrigger && !ce.isTrigger)
                {
                    Physics2D.IgnoreCollision(cj, ce, true);
                }
            }
        }
    }
    
}
