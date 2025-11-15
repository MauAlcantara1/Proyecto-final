using UnityEngine;
using UnityEngine.UI;

public class SlidersVolumen : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("volumenMaster"))
        {
            slider.value = PlayerPrefs.GetFloat("volumenMaster");
        }
        else
        {
            slider.value = 0f; // tu valor inicial
        }
    }
}
