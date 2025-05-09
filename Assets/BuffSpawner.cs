// BuffSpawner.cs
// Renommé pour éviter le conflit de nom avec PowerUpMultiplier  
using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class BuffSpawner : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("GameObject du joueur (pour position de référence)")]
    public GameObject playerObject;
    [Tooltip("(Optionnel) Parent sous lequel organiser les clones")]  
    public Transform clonesContainer;
    [Tooltip("TextMeshPro 3D affichant la valeur actuelle")]
    public TextMeshPro hpText;

    [Header("Paramètres du PowerUp")]
    [Tooltip("Nombre de clones à générer de base")]  
    public int baseValue = 1;
    [Tooltip("Distance maximale autour du joueur pour spawn")]  
    public float spawnRadius = 2f;
    [Tooltip("Distance minimale entre deux clones et le joueur")]  
    public float minSpacing = 1f;
    [Tooltip("Tentes de générer un clone jusqu'à ce nombre d'essais")]
    public int maxSpawnAttempts = 20;

    [Header("Mouvement du PowerUp")]
    [Tooltip("Vitesse de déplacement constant du power-up")]  
    public float moveSpeed = 3f;

    private int currentValue;
    private Vector3 moveDirection;

    void Awake()
    {
        // Collider en trigger et Rigidbody cinématique
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Start()
    {
        // Initialisation
        currentValue = baseValue;
        moveDirection = transform.forward;

        if (hpText != null)
            hpText.text = currentValue.ToString();
        else
            Debug.LogError("[BuffSpawner] hpText non assigné !");

        if (playerObject == null)
            Debug.LogError("[BuffSpawner] playerObject non assigné !");
    }

    void Update()
    {
        // Avance en ligne droite
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerObject)
        {
            SpawnClones(playerObject.transform);
            Destroy(gameObject);
        }
    }

    void SpawnClones(Transform playerTransform)
    {
        Vector3 basePos = playerTransform.position;
        float spawnY = basePos.y;

        // Liste des positions occupées (incl. joueur)
        List<Vector3> occupied = new List<Vector3> { basePos };

        for (int i = 0; i < currentValue; i++)
        {
            Vector3 spawnPos = basePos;
            bool found = false;

            // Recherche d'une position libre
            for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
            {
                Vector2 offset = Random.insideUnitCircle * spawnRadius;
                Vector3 candidate = new Vector3(basePos.x + offset.x, spawnY, basePos.z + offset.y);

                // Vérifie l'espacement
                bool tooClose = false;
                foreach (var occ in occupied)
                {
                    if (Vector3.Distance(occ, candidate) < minSpacing)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    spawnPos = candidate;
                    occupied.Add(spawnPos);
                    found = true;
                    break;
                }
            }

            // Si aucune position idéale trouvée, fallback autour du joueur
            if (!found)
            {
                Vector2 offset = Random.insideUnitCircle * spawnRadius;
                spawnPos = new Vector3(basePos.x + offset.x, spawnY, basePos.z + offset.y);
            }

            // Instanciation et organisation
            GameObject clone = Instantiate(playerObject, spawnPos, playerTransform.rotation);
            if (clonesContainer != null)
                clone.transform.SetParent(clonesContainer, worldPositionStays: true);

            // Tag pour séparation
            clone.tag = "Clone";
        }

        Debug.Log($"[BuffSpawner] Spawned {currentValue} clones spaced at >={minSpacing} units.");
    }
}
