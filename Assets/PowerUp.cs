using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PowerUp : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Référence automatique du joueur")]
    private PlayerShooting shooterScript;
    [Tooltip("TextMeshPro affichant les PV restants")]
    public TextMeshPro hpText;

    [Header("Paramètres du PowerUp")]
    [Tooltip("Points de vie initiaux du buff")]
    public int maxHealth = 3;
    [Tooltip("Bonus d'attackSpeed donné au joueur")]
    public float attackSpeedBonus = 0.5f;
    [Tooltip("Vitesse de déplacement")]
    public float moveSpeed = 3f;

    private int currentHealth;
    private Vector3 moveDirection;

    void Start()
    {
        // Initialisation
        currentHealth = maxHealth;
        moveDirection = transform.forward * -1f;

        // Utilisation de FindFirstObjectByType<T>() au lieu de FindObjectOfType<T>()
        shooterScript = FindFirstObjectByType<PlayerShooting>();

        if (shooterScript == null)
            Debug.LogError("[PowerUp] Aucun PlayerShooting trouvé !");

        // Vérification de l'UI
        if (hpText != null)
            hpText.text = currentHealth.ToString();
        else
            Debug.LogError("[PowerUp] hpText non assigné !");

        // Configuration Collider & Rigidbody
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        // Déplacement constant
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PowerUp] Collision détectée avec {other.gameObject.name}");

        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            currentHealth--;
            UpdateUIText();

            if (currentHealth <= 0)
                ApplyBuffAndDestroy();
        }
        else if (other.CompareTag("Player"))
        {
            ApplyBuffAndDestroy();
        }
    }

    private void UpdateUIText()
    {
        if (hpText != null)
            hpText.text = currentHealth.ToString();
    }

    private void ApplyBuffAndDestroy()
    {
        if (shooterScript == null)
            shooterScript = FindFirstObjectByType<PlayerShooting>();

        if (shooterScript != null)
        {
            shooterScript.attackSpeed += attackSpeedBonus;
            Debug.Log($"[PowerUp] attackSpeed augmenté à {shooterScript.attackSpeed:F2}");
        }
        else
        {
            Debug.LogError("[PowerUp] Impossible de trouver PlayerShooting !");
        }

        Destroy(gameObject);
    }
}
