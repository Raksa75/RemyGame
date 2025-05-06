using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SeparationBehaviour : MonoBehaviour
{
    [Header("S�paration (clones only)")]
    [Tooltip("Rayon de d�tection des autres clones")]
    public float separationRadius = 1f;
    [Tooltip("Force de r�pulsion")]
    public float separationStrength = 1f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Emp�che toute rotation pour rester droit
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        // On ne s�pare que si c'est bien un clone
        if (!CompareTag("Clone")) return;

        Vector3 force = Vector3.zero;
        int count = 0;

        // R�cup�re tous les colliders (y compris triggers) dans le rayon
        Collider[] hits = Physics.OverlapSphere(transform.position, separationRadius);
        foreach (var hit in hits)
        {
            // V�rifie que c'est un autre clone avec rigidbody
            if (hit.CompareTag("Clone") && hit.attachedRigidbody != null && hit.gameObject != gameObject)
            {
                Vector3 diff = transform.position - hit.transform.position;
                float dist = diff.magnitude;
                if (dist > 0f)
                {
                    // Pousse plus fort si plus proche
                    force += diff.normalized / dist;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            // Moyenne, normalisation, puis force
            force = (force / count).normalized * separationStrength;
            // VelocityChange pour un impact instantan�, sans tenir compte de la masse
            rb.AddForce(force, ForceMode.VelocityChange);
        }
    }

    // Utile pour voir le rayon dans la sc�ne
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
