using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    [Header("Vitesse du d�placement")]
    [Tooltip("Vitesse � laquelle l'ennemi avance en ligne droite")]
    public float moveSpeed = 3f;

    [Header("Direction de d�placement")]
    [Tooltip("Direction locale (dans l'espace du monde) dans laquelle l'ennemi avance")]
    public Vector3 moveDirection = Vector3.forward;

    void Start()
    {
        // Calcule la direction initiale selon l'orientation de l'objet
        moveDirection = transform.forward * -1f;

        // V�rifie qu'un Collider en mode trigger est pr�sent
        var col = GetComponent<Collider>();
        if (!col.isTrigger)
            Debug.LogWarning("[Enemy] Le Collider devrait �tre en mode Is Trigger pour d�tecter les collisions par OnTrigger.");
    }

    void Update()
    {
        // D�placement constant en espace monde
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        // Si on touche un joueur ou un clone, on le d�truit
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        // Si on est touch� par une balle, on d�truit la balle et l'ennemi
        else if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}