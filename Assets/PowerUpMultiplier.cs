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
    [Tooltip("TextMeshPro affichant la valeur actuelle")]
    public TextMeshPro hpText;

    [Header("Mouvement du PowerUp")]
    [Tooltip("Vitesse de déplacement")]
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
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Start()
    {
        currentValue = baseValue;

        if (hpText != null)
            hpText.text = currentValue.ToString();

        moveDirection = -transform.forward;

        if (playerPrefab == null)
            Debug.LogError("[PowerUpMultiplier] playerPrefab non assigné !");
    }

    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PowerUpMultiplier] Collision détectée avec {other.gameObject.name}");

        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            currentValue++;
            if (hpText != null)
                hpText.text = currentValue.ToString();
        }
        else if (other.CompareTag("Player"))
        {
            Debug.Log("[PowerUpMultiplier] Activation du buff de multiplication !");
            SpawnClones(other.transform);
            Debug.Log("[PowerUpMultiplier] Suppression du PowerUp après activation.");
            Destroy(gameObject);
        }
    }

    private void SpawnClones(Transform playerTransform)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[PowerUpMultiplier] playerPrefab n'est pas assigné !");
            return;
        }

        float checkRadius = spawnCheckRadius;
        var cc = playerPrefab.GetComponent<CharacterController>();
        if (cc != null)
            checkRadius = Mathf.Max(checkRadius, cc.radius);

        for (int i = 0; i < currentValue; i++)
        {
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

            GameObject clone = Instantiate(playerPrefab, spawnPos, playerTransform.rotation);
            if (clonesContainer != null)
                clone.transform.SetParent(clonesContainer, true);

            clone.tag = "Clone";

            Collider[] cloneCols = clone.GetComponentsInChildren<Collider>();
            Collider[] playerCols = playerTransform.GetComponentsInChildren<Collider>();

            foreach (var c in cloneCols)
                foreach (var pc in playerCols)
                    Physics.IgnoreCollision(c, pc);

            Debug.Log($"[PowerUpMultiplier] Clone {i + 1} spawned at {spawnPos}");
        }
    }
}
