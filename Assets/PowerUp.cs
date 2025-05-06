using UnityEngine;
using TMPro;

public class PowerUp : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Référence au script PlayerShooting du joueur (à assigner dans l'Inspector)")]
    public PlayerShooting shooter;

    [Header("Mouvement")]
    [Tooltip("Vitesse de déplacement dans la direction initiale")]
    public float moveSpeed = 3f;
    private Vector3 moveDirection;

    [Header("Vie")]
    [Tooltip("Points de vie maximum (entier)")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Bonus")]
    [Tooltip("Bonus d'attackSpeed à donner au joueur à la destruction")]
    public float attackSpeedBonus = 0.5f;

    [Header("UI Texte HP")]
    [Tooltip("Référence au TextMeshPro 3D qui affichera simplement le nombre de HP")]
    public TextMeshPro hpText;

    void Start()
    {
        // Initialisation de la vie et du mouvement
        currentHealth = maxHealth;
        moveDirection = transform.forward * -1f;

        // Vérifie que le TextMeshPro est bien assigné
        if (hpText == null)
            Debug.LogError("[PowerUp] hpText non assigné !");
        else
            UpdateHPText();

        // Vérifie la référence au shooter
        if (shooter == null)
            Debug.LogError("[PowerUp] shooter (PlayerShooting) non assigné !");
    }

    void Update()
    {
        // Avance tout droit
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
        }
    }

    void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        UpdateHPText();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateHPText()
    {
        // Affiche uniquement le nombre de HP restants
        hpText.text = currentHealth.ToString();
    }

    void Die()
    {
        // Applique le bonus si la référence est correcte
        if (shooter != null)
        {
            shooter.attackSpeed += attackSpeedBonus;
            Debug.Log($"[PowerUp] attackSpeed augmenté : {shooter.attackSpeed:F2}");
        }
        else
        {
            Debug.LogError("[PowerUp] Impossible d'appliquer le bonus : shooter non assigné.");
        }

        Destroy(gameObject);
    }
}
