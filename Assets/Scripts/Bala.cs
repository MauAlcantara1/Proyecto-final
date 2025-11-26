using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Propiedades de la bala")]
    [SerializeField] private float velocidad = 10f;
    [SerializeField] private int dano = 20;
    [SerializeField] private float tiempoVida = 2f;

    [Header("Dirección del disparo")]
    public Vector2 direccion = Vector2.right;

    private void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    private void Update()
    {
        transform.Translate(direccion * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstaculo") || other.CompareTag("Suelo"))
        {
            Debug.Log("Entró con obstáculo");
            Destroy(gameObject);
        }

        if (other.CompareTag("enemigo"))
        {
            bool impacto = false;

            EnemOso oso = other.GetComponent<EnemOso>();
            if (oso != null)
            {
                oso.RecibirDanio(dano);
                impacto = true;
            }

            // --- Tanque ---
            Tanque tanque = other.GetComponent<Tanque>();
            if (tanque != null)
            {
                tanque.RecibirDaño(dano);
                impacto = true;
            }

            Tanque2 tanque2 = other.GetComponent<Tanque2>();
            if (tanque2 != null)
            {
                tanque2.RecibirDaño(dano);
                impacto = true;
            }

            EnemYeti enemYeti = other.GetComponent<EnemYeti>();
            if (enemYeti != null)
            {
                enemYeti.RecibirDaño(dano);
                impacto = true;
            }

            EnemigoEscudero escudero = other.GetComponent<EnemigoEscudero>();
            if (escudero != null)
            {
                escudero.RecibirDaño(dano);
                impacto = true;
            }

            Soldadouno soldado = other.GetComponent<Soldadouno>();
            if (soldado != null)
            {
                soldado.RecibirDaño(dano);
                impacto = true;
            }

            if (impacto)
                Destroy(gameObject);
        }
    }
}
