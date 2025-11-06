using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disparo : MonoBehaviour
{
    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private GameObject bala;

    [SerializeField] private AudioSource audioSource;   // <-- nuevo
    [SerializeField] private AudioClip sfxDisparo;      // <-- nuevo

    private void Disparar()
    {
        Instantiate(bala, controladorDisparo.position, controladorDisparo.rotation);

        // reproducir sonido
        if (audioSource != null && sfxDisparo != null)
        {
            audioSource.PlayOneShot(sfxDisparo);
        }
    }
}
