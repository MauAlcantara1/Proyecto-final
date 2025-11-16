using UnityEngine;

public class musicaNivel : MonoBehaviour
{


    [SerializeField] private AudioClip musica; 
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(musica);
    }
    public void PausarMusica()
    {
        audioSource.Pause();
    }

    public void ReanudarMusica()
    {
        audioSource.UnPause();
    }

}
