using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HorizontalMovementController : MonoBehaviour
{
    [Header("Paramètres de déplacement")]
    [Tooltip("Vitesse de déplacement sur l'axe X")]
    public float speed = 5f;
    [Tooltip("LayerMask pour détecter uniquement les murs/obstacles")]
    public LayerMask environmentMask;
    [Tooltip("Distance tampon devant le personnage pour la détection")]
    public float wallCheckBuffer = 0.05f;

    private CharacterController cc;
    // -1 = bloqué à gauche, +1 = bloqué à droite, 0 = pas bloqué
    private float blockedDir = 0f;

    void Start()
    {
        CharacterManager.Instance?.RegisterCharacter(gameObject);
    }


    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1) Lecture de l’input horizontal
        float h = Input.GetAxis("Horizontal");
        if (Mathf.Abs(h) < 0.01f)
            return;

        float sign = Mathf.Sign(h);

        // 2) Si déjà bloqué dans ce sens, on n’essaie pas
        if (blockedDir != 0f && sign == blockedDir)
            return;

        // 3) Si on change de sens, on débloque
        if (blockedDir != 0f && sign == -blockedDir)
            blockedDir = 0f;

        // 4) Prépare le mouvement
        Vector3 direction = new Vector3(h, 0f, 0f).normalized;
        float moveDist = speed * Time.deltaTime;

        // Origine du raycast au centre du CharacterController
        Vector3 origin = transform.TransformPoint(cc.center);

        // 5) Raycast avant pour vérifier la présence d’un mur
        if (Physics.Raycast(origin, direction, moveDist + wallCheckBuffer, environmentMask))
        {
            blockedDir = sign;
            Debug.Log($"[HorizontalMovement] Bloqué côté {(sign > 0 ? "droite" : "gauche")}");
            return;
        }

        // 6) Aucun mur détecté, on bouge et on débloque
        blockedDir = 0f;
        cc.Move(direction * moveDist);
    }

    void OnDestroy()
    {
        CharacterManager.Instance?.RemoveCharacter(gameObject);
    }

}
