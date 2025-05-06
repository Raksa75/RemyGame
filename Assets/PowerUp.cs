using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PowerUp : MonoBehaviour
{
    [Header("R�f�rences (� assigner dans l'Inspector)")]
    [Tooltip("GameObject du joueur (celui portant le script de tir)")]
    public GameObject playerObject;
    [Tooltip("TextMeshPro 3D qui affichera les PV restants")]
    public TextMeshPro hpText;

    [Header("Param�tres du PowerUp")]
    [Tooltip("PV initiaux")]
    public int maxHealth = 3;
    [Tooltip("Bonus d'attackSpeed donn� au joueur")]
    public float attackSpeedBonus = 0.5f;
    [Tooltip("Vitesse de d�placement en ligne droite")]
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
            Debug.LogError("[PowerUp] hpText non assign� !");

        // R�cup�re le script PlayerShooting depuis le GameObject assign�
        if (playerObject != null)
        {
            shooterScript = playerObject.GetComponent<PlayerShooting>();
            if (shooterScript == null)
                Debug.LogError("[PowerUp] Aucun PlayerShooting trouv� sur playerObject !");
        }
        else
        {
            Debug.LogError("[PowerUp] playerObject non assign� !");
        }

        // Configure Collider et Rigidbody
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        // D�placement constant en world space
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
            Debug.Log($"[PowerUp] attackSpeed augment� � {shooterScript.attackSpeed:F2}");
        }
        Destroy(gameObject);
    }
}
