using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DangerObject : MonoBehaviour
{
    [Header("Param�tres de l�objet")]
    [Tooltip("Vitesse de d�placement")]
    public float moveSpeed = 3f;
    [Tooltip("Tag du joueur pour la d�tection")]
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
            Debug.LogError($"[DangerObject] Le joueur a �t� touch� ! GAME OVER");
            Destroy(other.gameObject); // Supprime le joueur
        }
    }
}
