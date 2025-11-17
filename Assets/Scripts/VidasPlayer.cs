using UnityEngine;

public class VidasPlayer : MonoBehaviour
{
    public static int vidas = 2;
    public static int puntuacion = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
