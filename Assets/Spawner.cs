using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Param�tres du spawn")]
    [Tooltip("Prefab � faire appara�tre")]
    public GameObject objectPrefab;
    [Tooltip("Intervalle entre chaque spawn (secondes)")]
    public float spawnInterval = 3f;
    [Tooltip("Position al�atoire du spawn")]
    public Vector3 spawnOffsetRange = new Vector3(1f, 0f, 1f);

    private void Start()
    {
        InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval);
    }

    private void SpawnObject()
    {
        if (objectPrefab == null)
        {
            Debug.LogError("[Spawner] Aucun prefab assign� !");
            return;
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnOffsetRange.x, spawnOffsetRange.x),
            Random.Range(-spawnOffsetRange.y, spawnOffsetRange.y),
            Random.Range(-spawnOffsetRange.z, spawnOffsetRange.z)
        );

        Vector3 spawnPos = transform.position + randomOffset;
        Instantiate(objectPrefab, spawnPos, Quaternion.identity);

        Debug.Log($"[Spawner] Objet spawn� � {spawnPos}");
    }
}
