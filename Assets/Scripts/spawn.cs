using UnityEngine;
using System.Collections;

public class spawn : MonoBehaviour
{
    [Header("Prefabs de enemigos")]
    public GameObject[] prefabsEnemigos; // ← Aquí metes tus 5 enemigos

    [Header("Puntos de spawn")]
    public Transform[] puntosSpawn;

    [Header("Oleadas")]
    public int numeroOleada = 1;
    public float tiempoEntreOleadas = 5f;
    public float tiempoEntreEnemigos = 0.5f;

    void Start()
    {
        StartCoroutine(SistemaOleadas());
    }

    IEnumerator SistemaOleadas()
    {
        while (true)
        {
            int cantidad = numeroOleada * 3;

            for (int i = 0; i < cantidad; i++)
            {
                Spawn();
                yield return new WaitForSeconds(tiempoEntreEnemigos);
            }

            numeroOleada++;
            yield return new WaitForSeconds(tiempoEntreOleadas);
        }
    }

    void Spawn()
    {
        if (prefabsEnemigos.Length == 0 || puntosSpawn.Length == 0)
            return;

        // Enemy aleatorio entre los 5
        GameObject enemigo = prefabsEnemigos[Random.Range(0, prefabsEnemigos.Length)];

        // Punto aleatorio
        Transform punto = puntosSpawn[Random.Range(0, puntosSpawn.Length)];

        Instantiate(enemigo, punto.position, punto.rotation);
    }
}
