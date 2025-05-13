using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PowerUp : MonoBehaviour
{
    [Header("Références")]
    private GroupManager gm;
    [Tooltip("TextMeshPro affichant les PV restants")]
    public TextMeshPro hpText;

    [Header("Paramètres du PowerUp")]
    [Tooltip("PV initiaux du buff")]
    public int maxHealth = 3;
    [Tooltip("Bonus d'attackSpeed appliqué à tous les clones (divisé par 3 pour le nerf)")]
    public float attackSpeedBonus = 0.5f / 3f;
    [Tooltip("Vitesse de déplacement")]
    public float moveSpeed = 3f;

    private int currentHealth;
    private Vector3 moveDirection;

    void Start()
    {
        currentHealth = maxHealth;
        moveDirection = transform.forward * -1f;

        gm = FindFirstObjectByType<GroupManager>();

        if (gm == null)
            Debug.LogError("[PowerUp] Aucun GroupManager trouvé !");

        if (hpText != null)
            hpText.text = currentHealth.ToString();
        else
            Debug.LogError("[PowerUp] hpText non assigné !");

        var col = GetComponent<Collider>();
        col.isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PowerUp] Collision détectée avec {other.gameObject.name}");

        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            currentHealth--;
            if (hpText != null)
                hpText.text = currentHealth.ToString();

            if (currentHealth <= 0)
                ApplyBuffAndDestroy();
        }
        else if (other.CompareTag("Player"))
        {
            ApplyBuffAndDestroy();
        }
    }

    private void ApplyBuffAndDestroy()
    {
        if (gm == null)
            gm = FindFirstObjectByType<GroupManager>();

        if (gm != null)
        {
            foreach (GameObject player in gm.players)
            {
                PlayerShooting shooter = player.GetComponent<PlayerShooting>();
                if (shooter != null)
                {
                    shooter.attackSpeed += attackSpeedBonus;
                }
            }
            Debug.Log($"[PowerUp] Buff nerfé appliqué à {gm.players.Count} personnages.");
        }
        else
        {
            Debug.LogError("[PowerUp] Impossible de trouver GroupManager !");
        }

        Destroy(gameObject);
    }
}
