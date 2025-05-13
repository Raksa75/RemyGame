using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DangerObject : MonoBehaviour
{
    [Header("Paramètres de l’objet")]
    [Tooltip("Vitesse de déplacement")]
    public float moveSpeed = 3f;
    [Tooltip("Tag du joueur pour la détection")]
    public string playerTag = "Player";

    private Vector3 moveDirection;

    void Start()
    {
        moveDirection = -transform.forward;
    }

    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.LogError($"[DangerObject] Le joueur a été touché ! GAME OVER");
            Destroy(other.gameObject); // Supprime le joueur
        }
    }
}
