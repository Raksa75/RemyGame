using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("R�f�rences")]
    [Tooltip("Prefab de la balle � instancier")]
    public GameObject bulletPrefab;
    [Tooltip("Point d'�mission des balles (vide GameObject)")]
    public Transform firePoint;

    [Header("Param�tres de tir")]
    [Tooltip("Cadence de tir (nombre de tirs par seconde)")]
    public float attackSpeed = 2f;
    [Tooltip("Vitesse initiale de la balle")]
    public float bulletSpeed = 20f;

    private float cooldownTimer = 0f;

    void Update()
    {
        // D�cr�mente le timer
        cooldownTimer -= Time.deltaTime;

        // Si le cooldown est �coul�, on tire et on r�initialise le timer
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
            rb.linearVelocity = firePoint.forward * bulletSpeed;
    }
}
