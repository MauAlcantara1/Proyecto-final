using UnityEngine;

public class GolpeJugador : MonoBehaviour
{
    [Header("Configuración del ataque")]
    [SerializeField] private int dano = 5;
    [SerializeField] private Collider2D colliderGolpe;

    private bool activo = false;

    void Start()
    {
        if (colliderGolpe != null)
            colliderGolpe.enabled = false;
    }

    // Evento de animación: activar el golpe
    public void ActivarGolpe()
    {
        if (colliderGolpe != null)
        {
            colliderGolpe.enabled = true;
            activo = true;
        }
    }

    // Evento de animación: desactivar el golpe
    public void DesactivarGolpe()
    {
        if (colliderGolpe != null)
        {
            colliderGolpe.enabled = false;
            activo = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activo) return;

        if (other.CompareTag("enemigo"))
        {
            // Dañar enemigo
            EnemOso oso = other.GetComponent<EnemOso>();
            if (oso != null)
            {
                oso.RecibirDanio(dano);
                Debug.Log($"[GOLPE] 💥 Impacto al Oso. Daño: {dano}");
            }

            Tanque tanque = other.GetComponent<Tanque>();
            if (tanque != null)
            {
                tanque.RecibirDaño(dano);
                Debug.Log($"[GOLPE] 💥 Impacto al Tanque. Daño: {dano}");
            }
        }
    }
}
