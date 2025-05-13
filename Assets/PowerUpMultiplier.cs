using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PowerUpMultiplier : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Prefab du personnage à dupliquer")]
    public GameObject playerPrefab;
    [Tooltip("(Optionnel) Container pour organiser les clones")]
    public Transform clonesContainer;
    [Tooltip("TextMeshPro affichant la valeur actuelle")]
    public TextMeshPro hpText;
    private GroupManager gm;

    [Header("Mouvement du PowerUp")]
    [Tooltip("Vitesse de déplacement")]
    public float moveSpeed = 3f;
    private Vector3 moveDirection;

    [Header("Paramètres du PowerUp")]
    [Tooltip("Valeur de base au lancement (nerf : divisé par 3)")]
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
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Start()
    {
        gm = FindFirstObjectByType<GroupManager>();
        currentValue = baseValue / 3;

        if (hpText != null)
            hpText.text = currentValue.ToString();

        moveDirection = -transform.forward;

        if (playerPrefab == null)
            Debug.LogError("[PowerUpMultiplier] playerPrefab non assigné !");
    }

    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Mise à jour dynamique du texte du buff
        UpdateBuffText();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PowerUpMultiplier] Collision détectée avec {other.gameObject.name}");

        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            currentValue++;
            UpdateBuffText();
        }
        else if (other.CompareTag("Player"))
        {
            // **Correction** : Ignorer la collision entre le joueur et le buff immédiatement
            foreach (var bonusCollider in GetComponentsInChildren<Collider>())
            {
                foreach (var playerCollider in other.GetComponentsInChildren<Collider>())
                {
                    Physics.IgnoreCollision(bonusCollider, playerCollider, true);
                }
            }

            ApplyBuffAndDestroy(other.transform);
        }
    }

    private void UpdateBuffText()
    {
        if (hpText != null)
        {
            hpText.text = currentValue.ToString();
            hpText.ForceMeshUpdate();
        }
    }

    private void ApplyBuffAndDestroy(Transform playerTransform)
    {
        if (gm == null)
            gm = FindFirstObjectByType<GroupManager>();

        if (gm != null)
        {
            int totalClones = Mathf.Max(1, currentValue / 3);

            Debug.Log($"[PowerUpMultiplier] Spawn de {totalClones} clones nerfés.");

            for (int i = 0; i < totalClones; i++)
            {
                SpawnClone(playerTransform);
            }
        }
        else
        {
            Debug.LogError("[PowerUpMultiplier] Impossible de trouver GroupManager !");
        }

        // **Correction** : Stabilise la position du joueur au sol après avoir pris le buff
        StartCoroutine(PlacePlayerOnGround(playerTransform));

        Destroy(gameObject);
    }

    private IEnumerator PlacePlayerOnGround(Transform playerTransform)
    {
        yield return new WaitForSeconds(0.1f);

        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position, Vector3.down, out hit, 3f, environmentMask))
        {
            playerTransform.position = hit.point;
        }
    }

    private void SpawnClone(Transform playerTransform)
    {
        if (playerPrefab == null) return;

        float checkRadius = spawnCheckRadius;
        var cc = playerPrefab.GetComponent<CharacterController>();
        if (cc != null)
            checkRadius = Mathf.Max(checkRadius, cc.radius);

        Vector3 spawnPos = playerTransform.position;
        bool found = false;

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 candidate = playerTransform.position + new Vector3(offset.x, 0f, offset.y);

            if (!Physics.CheckSphere(candidate, checkRadius, environmentMask, QueryTriggerInteraction.Ignore))
            {
                spawnPos = candidate;
                found = true;
                break;
            }
        }

        if (!found)
            spawnPos = playerTransform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));

        // **Correction** : Décaler la hauteur pour éviter la téléportation brutale
        spawnPos.y -= 0.5f;

        GameObject clone = Instantiate(playerPrefab, spawnPos, playerTransform.rotation);
        if (clonesContainer != null)
            clone.transform.SetParent(clonesContainer, true);

        clone.tag = "Clone";

        // **Ignore temporairement les collisions entre le joueur et le clone**
        foreach (var cloneCol in clone.GetComponentsInChildren<Collider>())
        {
            foreach (var playerCol in playerTransform.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(cloneCol, playerCol, true);
            }
        }

        Debug.Log($"[PowerUpMultiplier] Clone spawné à {spawnPos}");
    }
}
