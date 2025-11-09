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
        // Si impacta contra un enemigo
        if (other.CompareTag("enemigo"))
        {
            EnemOso oso = other.GetComponent<EnemOso>();

            if (oso != null)
            {
                oso.RecibirDanio(dano);
                Debug.Log($"[BALA] 💥 Impacto al oso. Daño enviado: {dano}");
            }
            else
            {
                Debug.LogWarning("[BALA] El objeto con tag 'enemigo' no tiene componente EnemOso.");
            }

            Destroy(gameObject); // Destruye la bala al impactar
        }
    }
}
