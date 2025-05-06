using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HorizontalMovementController : MonoBehaviour
{
    [Header("Paramètres")]
    [Tooltip("Vitesse de déplacement sur l'axe X")]
    public float speed = 5f;
    [Tooltip("Mask de l'environnement (murs, obstacles) utilisé pour CheckSphere si besoin)")]
    public LayerMask environmentMask;

    private CharacterController cc;

    // -1 = bloqué à gauche, +1 = bloqué à droite, 0 = pas bloqué
    private static float blockedDir = 0f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1) Lecture input horizontal uniquement
        float h = Input.GetAxis("Horizontal");
        if (Mathf.Abs(h) < 0.01f)
            return;

        // 2) Si on est bloqué dans cette direction, on ne bouge pas
        float sign = Mathf.Sign(h);
        if (blockedDir != 0f && sign == blockedDir)
            return;
        // si on va en sens inverse, on débloque
        if (blockedDir != 0f && sign == -blockedDir)
            blockedDir = 0f;

        // 3) Tente le déplacement
        Vector3 move = new Vector3(h, 0f, 0f);
        CollisionFlags flags = cc.Move(move.normalized * speed * Time.deltaTime);

        // 4) Si collision latérale, on bloque ce sens
        if ((flags & CollisionFlags.Sides) != 0)
        {
            blockedDir = sign;
            Debug.Log($"[HorizontalMovement] Bloqué côté {(sign > 0 ? "droite" : "gauche")}");
        }
    }
}
