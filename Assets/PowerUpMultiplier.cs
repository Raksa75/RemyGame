// PowerUpMultiplier.cs
// Ajout de l’ignorance des collisions entre clones et joueur/entre clones  
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PowerUpMultiplier : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Prefab du personnage à dupliquer")]
    public GameObject playerPrefab;
    [Tooltip("(Optionnel) Container pour organiser les clones")]
    public Transform clonesContainer;
    [Tooltip("TextMeshPro 3D qui affichera la valeur actuelle")]
    public TextMeshPro hpText;

    [Header("Mouvement du PowerUp")]
    [Tooltip("Vitesse de déplacement dans la direction initiale")]
    public float moveSpeed = 3f;
    private Vector3 moveDirection;

    [Header("Paramètres du PowerUp")]
    [Tooltip("Valeur de base au lancement")]
    public int baseValue = 1;
    [Tooltip("Distance maximale de spawn autour du joueur")]
    public float spawnRadius = 1.5f;

    [Header("Validation de spawn")]
    [Tooltip("LayerMask de l'environnement (murs, obstacles)")]
    public LayerMask environmentMask;
    [Tooltip("Rayon pour vérifier l'espace libre au spawn")]
    public float spawnCheckRadius = 0.5f;
    [Tooltip("Nombre maximum de tentatives de position avant fallback")]
    public int maxSpawnAttempts = 10;

    private int currentValue;

    void Awake()
    {
        // Configure Collider en trigger et Rigidbody kinematic pour OnTriggerEnter
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Start()
    {
        // Initialisation de la valeur et de l'UI
        currentValue = baseValue;
        if (hpText == null)
            Debug.LogError("[PowerUpMultiplier] hpText non assigné !");
        else
            UpdateUIText();

        // Détermine la direction de déplacement initiale (forward local)
        moveDirection = transform.forward * -1f;

        // Vérifie les références essentielles
        if (playerPrefab == null)
            Debug.LogError("[PowerUpMultiplier] playerPrefab non assigné !");
    }

    void Update()
    {
        // Déplacement constant en world space
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

        // Récupère le CharacterController pour son rayon, si présent
        float checkRadius = spawnCheckRadius;
        var cc = playerPrefab.GetComponent<CharacterController>();
        if (cc != null)
            checkRadius = Mathf.Max(checkRadius, cc.radius);

        // Liste pour ignorer collisions
        var existingClones = clonesContainer != null ? clonesContainer.GetComponentsInChildren<Transform>() : new Transform[0];

        for (int i = 0; i < currentValue; i++)
        {
            Vector3 spawnPos = playerTransform.position;
            bool found = false;

            // Tente de trouver une position libre
            for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
            {
                Vector2 offset = Random.insideUnitCircle * spawnRadius;
                Vector3 candidate = playerTransform.position + new Vector3(offset.x, 0f, offset.y);
                // Vérifie qu'il n'y a pas d'obstacle
                if (!Physics.CheckSphere(candidate, checkRadius, environmentMask, QueryTriggerInteraction.Ignore))
                {
                    spawnPos = candidate;
                    found = true;
                    break;
                }
            }

            // Si pas trouvé de place, on utilise position autour
            if (!found)
                spawnPos = playerTransform.position + Random.onUnitSphere * spawnRadius;

            // Instanciation
            GameObject clone = Instantiate(playerPrefab, spawnPos, playerTransform.rotation);
            if (clonesContainer != null)
                clone.transform.SetParent(clonesContainer, true);

            // Tag & ignore collisions
            clone.tag = "Clone";
            foreach (Transform child in clone.transform)
                child.tag = "Clone";

            // Désactive collisions clone↔joueur
            Collider[] cloneCols = clone.GetComponentsInChildren<Collider>();
            Collider[] playerCols = playerTransform.GetComponentsInChildren<Collider>();
            foreach (var c in cloneCols)
                foreach (var pc in playerCols)
                    Physics.IgnoreCollision(c, pc);

            // Désactive collisions clone↔clone existants
            foreach (var t in existingClones)
            {
                if (t == clone.transform) continue;
                Collider[] otherCols = t.GetComponentsInChildren<Collider>();
                foreach (var c in cloneCols)
                    foreach (var oc in otherCols)
                        Physics.IgnoreCollision(c, oc);
            }

            // Hérite de l'attackSpeed du joueur original
            var shooterOriginal = playerTransform.GetComponent<PlayerShooting>();
            var shooterClone = clone.GetComponent<PlayerShooting>();
            if (shooterOriginal != null && shooterClone != null)
                shooterClone.attackSpeed = shooterOriginal.attackSpeed;
        }

        Debug.Log($"[PowerUpMultiplier] Spawned {currentValue} clone(s).");
    }
}
