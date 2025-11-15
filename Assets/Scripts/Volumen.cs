using UnityEngine;
using UnityEngine.Audio;

public class Volumen : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void CambiarVolumen(float volumen)
    {
        audioMixer.SetFloat("Volumen", volumen);
        PlayerPrefs.SetFloat("volumenMaster", volumen);
    }

}
