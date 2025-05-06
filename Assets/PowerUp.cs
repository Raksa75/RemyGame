using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PowerUp : MonoBehaviour
{
    [Header("Références (à assigner dans l'Inspector)")]
    [Tooltip("GameObject du joueur (celui portant le script de tir)")]
    public GameObject playerObject;
    [Tooltip("TextMeshPro 3D qui affichera les PV restants")]
    public TextMeshPro hpText;

    [Header("Paramètres du PowerUp")]
    [Tooltip("PV initiaux")]
    public int maxHealth = 3;
    [Tooltip("Bonus d'attackSpeed donné au joueur")]
    public float attackSpeedBonus = 0.5f;
    [Tooltip("Vitesse de déplacement en ligne droite")]
    public float moveSpeed = 3f;

    private int currentHealth;
    private Vector3 moveDirection;
    private PlayerShooting shooterScript;

    void Start()
    {
        // Initialise la vie et la direction
        currentHealth = maxHealth;
        moveDirection = transform.forward * -1f;

        // Configure UI
        if (hpText != null)
            hpText.text = currentHealth.ToString();
        else
            Debug.LogError("[PowerUp] hpText non assigné !");

        // Récupère le script PlayerShooting depuis le GameObject assigné
        if (playerObject != null)
        {
            shooterScript = playerObject.GetComponent<PlayerShooting>();
            if (shooterScript == null)
                Debug.LogError("[PowerUp] Aucun PlayerShooting trouvé sur playerObject !");
        }
        else
        {
            Debug.LogError("[PowerUp] playerObject non assigné !");
        }

        // Configure Collider et Rigidbody
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        // Déplacement constant en world space
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            currentHealth--;
            if (hpText != null)
                hpText.text = currentHealth.ToString();

            if (currentHealth <= 0)
                ApplyBuffAndDestroy();
        }
        else if (other.gameObject == playerObject)
        {
            ApplyBuffAndDestroy();
        }
    }

    private void ApplyBuffAndDestroy()
    {
        if (shooterScript != null)
        {
            shooterScript.attackSpeed += attackSpeedBonus;
            Debug.Log($"[PowerUp] attackSpeed augmenté à {shooterScript.attackSpeed:F2}");
        }
        Destroy(gameObject);
    }
}
