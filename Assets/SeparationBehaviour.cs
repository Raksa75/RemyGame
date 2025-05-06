using UnityEngine;

public class SeparationController : MonoBehaviour
{
    [Tooltip("Distance minimale entre clones")]
    public float desiredSeparation = 1f;
    [Tooltip("Force de séparation (units/sec)")]
    public float separationStrength = 2f;
    [Tooltip("LayerMask des clones seuls")]
    public LayerMask cloneLayer;
    [Tooltip("LayerMask de l'environnement (murs...)")]
    public LayerMask environmentMask;

    float radius;

    void Start()
    {
        // On récupère le rayon de collision via le collider
        radius = GetComponent<Collider>().bounds.extents.magnitude;
    }

    void FixedUpdate()
    {
        // 1) Récupère tous les clones dans le spectre
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            desiredSeparation,
            cloneLayer,
            QueryTriggerInteraction.Ignore
        );

        Vector3 move = Vector3.zero;
        int count = 0;
        foreach (var h in hits)
        {
            if (h.gameObject == gameObject) continue;
            float d = Vector3.Distance(transform.position, h.transform.position);
            if (d > 0f && d < desiredSeparation)
            {
                // Pousse proportionnel à la proximité
                move += (transform.position - h.transform.position).normalized * ((desiredSeparation - d) / desiredSeparation);
                count++;
            }
        }
        if (count == 0) return;

        // 2) Moyenne, normalisation puis échelle par la force et le time
        move = (move / count).normalized * separationStrength * Time.fixedDeltaTime;

        // 3) Ne bouge que si la nouvelle position est libre de murs
        Vector3 target = transform.position + move;
        if (!Physics.CheckSphere(target, radius, environmentMask, QueryTriggerInteraction.Ignore))
        {
            transform.position = target;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, desiredSeparation);
    }
}
