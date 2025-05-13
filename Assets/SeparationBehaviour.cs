using UnityEngine;

public class SeparationBehaviour : MonoBehaviour
{
    [Tooltip("Distance minimale entre clones")]
    public float desiredSeparation = 1f;
    [Tooltip("Force de séparation")]
    public float separationStrength = 2f;
    [Tooltip("LayerMask des clones seuls")]
    public LayerMask cloneLayer;
    [Tooltip("LayerMask de l'environnement (murs...)")]
    public LayerMask environmentMask;

    float radius;

    void Start()
    {
        radius = GetComponent<Collider>().bounds.extents.magnitude;

        // Ajoute chaque clone au groupe dynamique
        GroupManager gm = FindFirstObjectByType<GroupManager>();
        if (gm != null)
            gm.RegisterPlayer(gameObject);
    }

    void FixedUpdate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, desiredSeparation, cloneLayer, QueryTriggerInteraction.Ignore);

        Vector3 move = Vector3.zero;
        int count = 0;
        foreach (var h in hits)
        {
            if (h.gameObject == gameObject) continue;
            float d = Vector3.Distance(transform.position, h.transform.position);
            if (d > 0f && d < desiredSeparation)
            {
                move += (transform.position - h.transform.position).normalized * ((desiredSeparation - d) / desiredSeparation);
                count++;
            }
        }

        if (count == 0) return;

        move = (move / count).normalized * separationStrength * Time.fixedDeltaTime;

        Vector3 target = transform.position + move;

        // Empêche la fusion et assure l’interaction avec les murs
        if (!Physics.CheckSphere(target, radius, environmentMask, QueryTriggerInteraction.Ignore))
        {
            transform.position = target;
        }
    }

    void OnDestroy()
    {
        GroupManager gm = FindFirstObjectByType<GroupManager>();
        if (gm != null)
            gm.RemovePlayer(gameObject);
    }
}
