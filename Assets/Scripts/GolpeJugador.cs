using UnityEngine;

public class GolpeJugador : MonoBehaviour
{
    [Header("Configuración del ataque")]
    [SerializeField] private int dano = 5;
    [SerializeField] private Collider2D colliderGolpe;

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("enemigo"))
        {
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

            Tanque2 tanque2 = other.GetComponent<Tanque2>();
            if (tanque2 != null)
            {
                tanque2.RecibirDaño(dano);
                Debug.Log($"[BALA] 💥 Impacto al Tanque2. Daño enviado: {dano}");
            }

            EnemYeti enemYeti = other.GetComponent<EnemYeti>();
            if (enemYeti != null)
            {
                enemYeti.RecibirDaño(dano);
                Debug.Log($"[BALA] 💥 Impacto al Yeti. Daño enviado: {dano}");
            }

            EnemigoEscudero escudero = other.GetComponent<EnemigoEscudero>();
            if (escudero != null)
            {
                escudero.RecibirDaño(dano);
                Debug.Log($"[BALA] 💥 Impacto al Escudero. Daño enviado: {dano}");
            }
        }
    }
}
