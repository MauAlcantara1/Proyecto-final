using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disparo : MonoBehaviour
{
    [SerializeField] private int dano = 3;

    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private GameObject bala;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfxDisparo;

    private void Disparar()
    {
        Instantiate(bala, controladorDisparo.position, controladorDisparo.rotation);

        // reproducir sonido
        if (audioSource != null && sfxDisparo != null)
        {
            audioSource.PlayOneShot(sfxDisparo);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("enemigo"))
        {
            bool impacto = false;

            EnemOso oso = other.GetComponent<EnemOso>();
            if (oso != null)
            {
                oso.RecibirDanio(dano);
                Debug.Log($"[cuerpo] 💥 Impacto al Oso. Daño enviado: {dano}");
                impacto = true;
            }

            Tanque tanque = other.GetComponent<Tanque>();
            if (tanque != null)
            {
                tanque.RecibirDaño(dano);
                impacto = true;
            }
        }
    }
}
