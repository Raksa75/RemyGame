using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class EnemySpawner : MonoBehaviour
{
    [Header("R�f�rences")]
    [Tooltip("Prefab de l'ennemi � instancier")]
    public GameObject enemyPrefab;

    [Header("Zone de spawn")]
    [Tooltip("BoxCollider d�finissant la zone de spawn (Non Trigger)")]
    private BoxCollider spawnZone;

    [Header("Param�tres de spawn")]
    [Tooltip("D�lai de base entre spawns quand pas d'ennemi")]
    public float baseSpawnInterval = 1f;
    [Tooltip("D�lai max entre spawns quand zone satur�e")]
    public float maxSpawnInterval = 5f;
    [Tooltip("Nombre max d'ennemis souhait� dans la zone (pour calcul de saturation)")]
    public int maxEnemies = 10;
    [Tooltip("Nombre maximal de tentatives pour trouver une position libre avant abandon")]
    public int maxSpawnAttempts = 10;

    [Header("Validation de spawn")]
    [Tooltip("Rayon utilis� pour v�rifier l'espace libre autour du point de spawn")]
    public float spawnCheckRadius = 0.5f;
    [Tooltip("LayerMask de tous les obstacles (ennemis, murs, etc.) � consid�rer pour la v�rification")]
    public LayerMask obstacleMask;

    void Awake()
    {
        spawnZone = GetComponent<BoxCollider>();
        if (spawnZone == null)
            Debug.LogError("[EnemySpawner] Aucun BoxCollider trouv� !");
        else if (spawnZone.isTrigger)
            Debug.LogWarning("[EnemySpawner] Le BoxCollider doit �tre non-Trigger pour d�finir la zone.");
    }

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Calcule l'intervalle dynamique selon saturation
            int currentCount = CountEnemiesInZone();
            float occupancy = Mathf.Clamp01((float)currentCount / maxEnemies);
            float interval = Mathf.Lerp(baseSpawnInterval, maxSpawnInterval, occupancy);

            yield return new WaitForSeconds(interval);
            TrySpawn(occupancy);
        }
    }

    private int CountEnemiesInZone()
    {
        // Comptage des ennemis (tag 'Enemy') dans la box
        Vector3 center = transform.position + spawnZone.center;
        Vector3 halfSize = spawnZone.size * 0.5f;
        Collider[] hits = Physics.OverlapBox(center, halfSize, Quaternion.identity, 1 << LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Ignore);
        int count = 0;
        foreach (var hit in hits)
            if (hit.CompareTag("Enemy"))
                count++;
        return count;
    }

    private void TrySpawn(float occupancy)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] Prefab d'ennemi non assign� !");
            return;
        }

        // Si d�j� au max, on skip
        if (occupancy >= 1f) return;

        // Calcul des extents de la zone
        Vector3 halfSize = spawnZone.size * 0.5f;
        Vector3 center = transform.position + spawnZone.center;

        // Tente plusieurs positions al�atoires
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-halfSize.x, halfSize.x),
                0f,
                Random.Range(-halfSize.z, halfSize.z)
            );
            Vector3 candidatePos = center + randomOffset;

            // V�rifie que l'espace est libre
            if (!Physics.CheckSphere(candidatePos, spawnCheckRadius, obstacleMask, QueryTriggerInteraction.Ignore))
            {
                Instantiate(enemyPrefab, candidatePos, Quaternion.identity);
                break;
            }
        }
    }
}