using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Prefab de la balle à instancier")]
    public GameObject bulletPrefab;
    [Tooltip("Point d'émission des balles (vide GameObject)")]
    public Transform firePoint;

    [Header("Paramètres de tir")]
    [Tooltip("Cadence de tir (nombre de tirs par seconde)")]
    public float attackSpeed = 2f;
    [Tooltip("Vitesse initiale de la balle")]
    public float bulletSpeed = 20f;

    private float cooldownTimer = 0f;

    void Update()
    {
        // Décrémente le timer
        cooldownTimer -= Time.deltaTime;

        // Si le cooldown est écoulé, on tire et on réinitialise le timer
        if (cooldownTimer <= 0f)
        {
            Shoot();
            cooldownTimer = 1f / attackSpeed;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        // Instanciation de la balle
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        // Impulsion initiale
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = firePoint.forward * bulletSpeed;
    }
}
