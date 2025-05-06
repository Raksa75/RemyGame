using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PowerUpMultiplier : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Prefab du personnage à dupliquer")]
    public GameObject playerPrefab;
    [Tooltip("Parent sous lequel on instancie les clones")]
    public Transform spawnParent;
    [Tooltip("Référence au TextMeshPro 3D qui affichera la valeur")]
    public TextMeshPro hpText;

    [Header("Mouvement")]
    [Tooltip("Vitesse de déplacement dans la direction initiale")]
    public float moveSpeed = 3f;
    private Vector3 moveDirection;

    [Header("Paramètres du PowerUp")]
    [Tooltip("Valeur de base")]
    public int baseValue = 1;
    [Tooltip("Distance de spawn des clones autour du joueur")]
    public float spawnRadius = 1.5f;

    private int currentValue;

    void Start()
    {
        // Initialisation
        currentValue = baseValue;
        UpdateUIText();

        // On garde la direction forward de départ pour un mouvement en ligne droite
        moveDirection = transform.forward * -1f;

        if (hpText == null) Debug.LogError("[PowerUpMultiplier] hpText non assigné !");
        if (playerPrefab == null) Debug.LogError("[PowerUpMultiplier] playerPrefab non assigné !");
        if (spawnParent == null) spawnParent = transform.parent;
    }

    void Update()
    {
        // Avance en ligne droite
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            currentValue += 1;
            UpdateUIText();
        }
        else if (other.CompareTag("Player"))
        {
            SpawnClones(other.transform);
            Destroy(gameObject);
        }
    }

    void UpdateUIText()
    {
        hpText.text = currentValue.ToString();
    }

    void SpawnClones(Transform playerTransform)
    {
        for (int i = 0; i < currentValue; i++)
        {
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = playerTransform.position + new Vector3(circle.x, 0f, circle.y);
            GameObject clone = Instantiate(playerPrefab, spawnPos, playerTransform.rotation, spawnParent);
            // Change son tag
            clone.tag = "Clone";
            // Si ton prefab a plusieurs objets enfants taggés “Player”
            // et que tu veux aussi les changer, fais par exemple :
            foreach (Transform child in clone.transform)
                child.tag = "Clone";
            // optionnel : hériter des stats du joueur original
            var shooting = clone.GetComponent<PlayerShooting>();
            if (shooting != null)
                shooting.attackSpeed = playerTransform.GetComponent<PlayerShooting>().attackSpeed;
        }

        Debug.Log($"[PowerUpMultiplier] Spawned {currentValue} clone(s).");
    }
}
