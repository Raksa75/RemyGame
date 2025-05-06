using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PowerUpMultiplier : MonoBehaviour
{
    [Header("R�f�rences")]
    [Tooltip("Prefab du personnage � dupliquer")]
    public GameObject playerPrefab;
    [Tooltip("(Optionnel) Container pour organiser les clones")]
    public Transform clonesContainer;
    [Tooltip("TextMeshPro 3D qui affichera la valeur actuelle")]
    public TextMeshPro hpText;

    [Header("Mouvement du PowerUp")]
    [Tooltip("Vitesse de d�placement dans la direction initiale")]
    public float moveSpeed = 3f;
    private Vector3 moveDirection;

    [Header("Param�tres du PowerUp")]
    [Tooltip("Valeur de base au lancement")]
    public int baseValue = 1;
    [Tooltip("Distance maximale de spawn autour du joueur")]
    public float spawnRadius = 1.5f;

    [Header("Validation de spawn")]
    [Tooltip("LayerMask de l'environnement (murs, obstacles)")]
    public LayerMask environmentMask;
    [Tooltip("Rayon pour v�rifier l'espace libre au spawn")]
    public float spawnCheckRadius = 0.5f;
    [Tooltip("Nombre maximum de tentatives de position avant fallback")]
    public int maxSpawnAttempts = 10;

    private int currentValue;

    void Start()
    {
        // Initialisation de la valeur et de l'UI
        currentValue = baseValue;
        if (hpText == null)
            Debug.LogError("[PowerUpMultiplier] hpText non assign� !");
        else
            UpdateUIText();

        // D�termine la direction de d�placement initiale (forward local)
        moveDirection = transform.forward * -1f;

        // V�rifie les r�f�rences essentielles
        if (playerPrefab == null)
            Debug.LogError("[PowerUpMultiplier] playerPrefab non assign� !");

        // Si aucun container d�fini, on ne parent pas les clones
    }

    void Update()
    {
        // D�placement constant en world space
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            currentValue++;
            UpdateUIText();
        }
        else if (other.CompareTag("Player"))
        {
            SpawnClones(other.transform);
            Destroy(gameObject);
        }
    }

    private void UpdateUIText()
    {
        if (hpText != null)
            hpText.text = currentValue.ToString();
    }

    private void SpawnClones(Transform playerTransform)
    {
        if (playerPrefab == null)
            return;

        // R�cup�re le CharacterController pour son rayon, si pr�sent
        float checkRadius = spawnCheckRadius;
        var cc = playerPrefab.GetComponent<CharacterController>();
        if (cc != null)
            checkRadius = Mathf.Max(checkRadius, cc.radius);

        for (int i = 0; i < currentValue; i++)
        {
            Vector3 spawnPos = playerTransform.position;
            bool found = false;

            // Tente de trouver une position libre
            for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
            {
                Vector2 offset = Random.insideUnitCircle * spawnRadius;
                Vector3 candidate = playerTransform.position + new Vector3(offset.x, 0f, offset.y);
                // V�rifie qu'il n'y a pas d'obstacle
                if (!Physics.CheckSphere(candidate, checkRadius, environmentMask, QueryTriggerInteraction.Ignore))
                {
                    spawnPos = candidate;
                    found = true;
                    break;
                }
            }

            // Si pas trouv� de place, on utilise position autour
            if (!found)
                spawnPos = playerTransform.position + Random.onUnitSphere * spawnRadius;

            // Instanciation
            GameObject clone = Instantiate(playerPrefab, spawnPos, playerTransform.rotation);
            // Organise sous un container si d�fini
            if (clonesContainer != null)
                clone.transform.SetParent(clonesContainer, true);

            // Tague le clone pour identification
            clone.tag = "Clone";
            foreach (Transform child in clone.transform)
                child.tag = "Clone";

            // H�rite de l'attackSpeed du joueur original
            var shooterOriginal = playerTransform.GetComponent<PlayerShooting>();
            var shooterClone = clone.GetComponent<PlayerShooting>();
            if (shooterOriginal != null && shooterClone != null)
                shooterClone.attackSpeed = shooterOriginal.attackSpeed;
        }

        Debug.Log($"[PowerUpMultiplier] Spawned {currentValue} clone(s).");
    }
}
