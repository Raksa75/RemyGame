using UnityEngine;

public class BuffSpawner : MonoBehaviour
{
    [Header("Buffs disponibles")]
    [Tooltip("Liste des prefabs de buffs")]
    public GameObject[] buffPrefabs;
    [Tooltip("Intervalle de spawn (nerfé)")]
    public float spawnInterval = 10f;
    [Tooltip("Chance de spawn d'un buff (ex: 50% = 0.5f)")]
    public float spawnChance = 0.5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnBuff), spawnInterval * 3f, spawnInterval * 3f); // Nerf du spawn
    }

    void SpawnBuff()
    {
        if (Random.value > spawnChance) // Vérifie la probabilité de spawn
        {
            Debug.Log("[BuffSpawner] Pas de buff cette fois !");
            return;
        }

        if (buffPrefabs.Length == 0)
        {
            Debug.LogError("[BuffSpawner] Aucun buff assigné !");
            return;
        }

        GameObject chosenBuff = buffPrefabs[Random.Range(0, buffPrefabs.Length)]; // Buff aléatoire
        Vector3 spawnPos = transform.position; // Spawn directement sur le spawner

        Instantiate(chosenBuff, spawnPos, Quaternion.identity);
        Debug.Log($"[BuffSpawner] Nouveau buff spawné : {chosenBuff.name}");
    }
}
