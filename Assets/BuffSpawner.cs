using UnityEngine;

public class BuffSpawner : MonoBehaviour
{
    public GameObject[] buffs; // Tableau contenant les buffs
    public float spawnInterval = 5f; // Temps entre chaque spawn
    public Transform spawnPoint; // Position de spawn

    void Start()
    {
        if (buffs.Length == 0 || spawnPoint == null)
        {
            Debug.LogError("[BuffSpawner] Aucun buff défini ou spawnPoint manquant !");
            return;
        }

        // Spawn immédiat du premier buff
        SpawnBuff();

        // Démarrage du spawn régulier
        InvokeRepeating(nameof(SpawnBuff), spawnInterval, spawnInterval);
    }

    void SpawnBuff()
    {
        if (buffs.Length == 0 || spawnPoint == null) return;

        int randomIndex = Random.Range(0, buffs.Length);
        GameObject newBuff = Instantiate(buffs[randomIndex], spawnPoint.position, Quaternion.identity);

        // Vérifie si le buff spawn un clone et l’ajoute au CharacterManager
        CharacterController clone = newBuff.GetComponent<CharacterController>();
        if (clone != null)
        {
            CharacterManager.Instance?.RegisterCharacter(newBuff);
        }
    }
}
