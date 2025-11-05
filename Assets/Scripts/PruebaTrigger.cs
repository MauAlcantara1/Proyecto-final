using UnityEngine;

public class PruebaTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log($"[DEBUG] OnTriggerEnter2D ACTIVADO con {other.name}, tag={other.tag}");
}

}
