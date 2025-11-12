using UnityEngine;

public class Balatanque : MonoBehaviour
{
    [Header("Propiedades")]
    public float velocidad = 10f;
    public float tiempoVida = 3f;
    public int daño = 1;

    void Start()
    {
        // Destruye la bala después de un tiempo para evitar acumular basura
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        // Mueve la bala hacia la dirección "derecha" local del prefab
        transform.Translate(Vector3.right * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si golpea al jugador, podrías aplicarle daño
        if (other.CompareTag("Player"))
        {
            Debug.Log("💥 Bala del tanque golpeó al jugador");

            // Si tu jugador tiene un script de salud, puedes hacer algo como:
            // other.GetComponent<Jugador>().RecibirDaño(daño);
        }

        // Destruye la bala al colisionar con cualquier cosa que no sea el tanque
        if (!other.CompareTag("enemigo"))
        {
            Destroy(gameObject);
        }
    }
}
