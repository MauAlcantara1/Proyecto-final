using UnityEngine;

/// <summary>
/// DropLoot simplificado: el objeto aparece en la posición exacta y no se mueve.
/// </summary>
public class DropLoot : MonoBehaviour
{
    [Header("Drops")]
    public GameObject[] posiblesDrops;

    [Header("Configuración")]
    [Range(0f, 1f)]
    public float probabilidadDrop = 1f; // siempre dropea para pruebas
    public int minDrops = 1;
    public int maxDrops = 1;

    public void SoltarObjetos()
    {
        if (posiblesDrops == null || posiblesDrops.Length == 0)
        {
            Debug.LogWarning("[DropLoot] No hay prefabs asignados para soltar.");
            return;
        }

        if (Random.value > probabilidadDrop)
        {
            Debug.Log("[DropLoot] No se cumple la probabilidad de drop.");
            return;
        }

        int cantidad = Random.Range(minDrops, maxDrops + 1);
        Debug.Log($"[DropLoot] Dropeando {cantidad} objeto(s).");

        for (int i = 0; i < cantidad; i++)
        {
            GameObject prefab = posiblesDrops[Random.Range(0, posiblesDrops.Length)];
            if (prefab == null) continue;

            Vector3 posicionSpawn = transform.position; // exactamente donde muere el oso
            GameObject inst = Instantiate(prefab, posicionSpawn, Quaternion.identity);

            Debug.Log($"[DropLoot] Spawn objeto: {prefab.name} en {posicionSpawn}");
        }
    }
}
