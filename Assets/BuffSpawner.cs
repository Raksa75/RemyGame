using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]
public class BuffSpawner : MonoBehaviour
{
    [Header("R�f�rences")]
    [Tooltip("Liste des prefabs de buffs � instancier (PowerUp, PowerUpMultiplier, etc.)")]
    public GameObject[] buffPrefabs;

    [Header("Param�tres de spawn")]
    [Tooltip("Intervalle en secondes entre chaque spawn de buff")]
    public float spawnInterval = 5f;
    [Tooltip("Distance al�atoire autour du spawner pour le placement (XZ)")]
    public float spawnRadius = 0f;

    void Start()
    {
        if (buffPrefabs == null || buffPrefabs.Length == 0)
        {
            Debug.LogError("[BuffSpawner] Aucun prefab de buff assign� !");
            enabled = false;
            return;
        }

        // D�marre la coroutine de spawn
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnBuff();
        }
    }

    private void SpawnBuff()
    {
        // Choix al�atoire d'un prefab
        int idx = Random.Range(0, buffPrefabs.Length);
        GameObject prefab = buffPrefabs[idx];

        // Calcul de la position de spawn (optionnellement al�atoire autour du spawner)
        Vector3 spawnPos = transform.position;
        if (spawnRadius > 0f)
        {
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            spawnPos += new Vector3(circle.x, 0f, circle.y);
        }

        // Instanciation du buff
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
