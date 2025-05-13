using UnityEngine;

public class BuffSpawner : MonoBehaviour
{
    [Header("Buffs disponibles")]
    [Tooltip("Liste des prefabs de buffs")]
    public GameObject[] buffPrefabs;
    [Tooltip("Probabilités de chaque buff (ex: 0.2 = 20%, 0.8 = 80%)")]
    public float[] buffProbabilities;
    [Tooltip("Intervalle de spawn (nerfé)")]
    public float spawnInterval = 10f;

    void Start()
    {
        if (buffPrefabs.Length != buffProbabilities.Length)
        {
            Debug.LogError("[BuffSpawner] Le nombre de buffs ne correspond pas au nombre de probabilités !");
            return;
        }

        InvokeRepeating(nameof(SpawnBuff), spawnInterval * 3f, spawnInterval * 3f); // Nerf du spawn
    }

    void SpawnBuff()
    {
        if (buffPrefabs.Length == 0)
        {
            Debug.LogError("[BuffSpawner] Aucun buff assigné !");
            return;
        }

        GameObject chosenBuff = ChooseBuffBasedOnProbability();
        if (chosenBuff == null)
        {
            Debug.Log("[BuffSpawner] Aucun buff ne spawn cette fois !");
            return;
        }

        Vector3 spawnPos = transform.position;
        Instantiate(chosenBuff, spawnPos, Quaternion.identity);
        Debug.Log($"[BuffSpawner] Nouveau buff spawné : {chosenBuff.name}");
    }

    GameObject ChooseBuffBasedOnProbability()
    {
        float randomValue = Random.value; // Valeur entre 0 et 1
        float cumulativeProbability = 0f;

        for (int i = 0; i < buffPrefabs.Length; i++)
        {
            cumulativeProbability += buffProbabilities[i];
            if (randomValue <= cumulativeProbability)
                return buffPrefabs[i];
        }

        return null; // Aucun buff ne spawn si aucune condition n'est remplie
    }
}
