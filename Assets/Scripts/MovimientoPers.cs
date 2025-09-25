using UnityEngine;

public class MovimientoPers : MonoBehaviour
{
    public float speed = 5f;        // Velocidad de movimiento
    public float jumpForce = 10f;   // Fuerza del salto
    private Rigidbody2D rb;

    private bool isGrounded = true; // Para saber si est� en el suelo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movimiento horizontal
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        // Agacharse (placeholder: solo escalarlo)
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(0.5f, 1f, 1f);
        }
    }

    // Detectar si toca el suelo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f) // Detecta si choca con algo debajo
        {
            isGrounded = true;
        }
    }
}
