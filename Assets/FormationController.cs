using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// FormationController vFinal :
/// - Répartit les clones en cercle ou en ligne devant un mur
/// - Leur position est gérée par NavMeshAgent pour des collisions et évitements réels
/// </summary>
public class FormationController : MonoBehaviour
{
    [Header("Clones")] 
    [Tooltip("Tag attribué aux clones")] 
    public string cloneTag = "Clone";

    [Header("Formation Cercle")]
    [Tooltip("Rayon du cercle de formation autour du joueur")] 
    public float circleRadius = 2f;

    [Header("Détection Mur")]
    [Tooltip("LayerMask pour détecter uniquement les murs")] 
    public LayerMask environmentMask;
    [Tooltip("Distance de détection de mur devant le joueur")] 
    public float wallDetectionDistance = 1f;

    void Update()
    {
        Vector3 playerPos = transform.position;
        GameObject[] clones = GameObject.FindGameObjectsWithTag(cloneTag);
        int count = clones.Length;
        if (count == 0) return;

        // Vérifie présence d'un mur
        bool nearWall = Physics.Raycast(
            playerPos, transform.forward, out RaycastHit hit, wallDetectionDistance, environmentMask, QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < count; i++)
        {
            var clone = clones[i];
            var agent = clone.GetComponent<NavMeshAgent>();
            if (agent == null) continue;

            Vector3 target;
            if (nearWall)
            {
                // Formation en ligne parallèle au mur
                Vector3 normal = hit.normal;
                Vector3 tangent = Vector3.Cross(normal, Vector3.up).normalized;
                float spacing = (circleRadius * 2f) / count;
                float offset = (i - (count - 1) * 0.5f) * spacing;
                // Point de référence devant le mur
                Vector3 wallCenter = hit.point + normal * agent.radius;
                target = wallCenter + tangent * offset;
            }
            else
            {
                // Formation en cercle
                float angle = i * Mathf.PI * 2f / count;
                target = playerPos + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * circleRadius;
            }

            agent.SetDestination(target);
        }
    }
}
