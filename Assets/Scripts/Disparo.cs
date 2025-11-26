using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disparo : MonoBehaviour
{
    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private GameObject bala;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfxDisparo;

    private void Disparar()
    {
        Instantiate(bala, controladorDisparo.position, controladorDisparo.rotation);

        if (audioSource != null && sfxDisparo != null)
        {
            audioSource.PlayOneShot(sfxDisparo);
        }
    }
}
