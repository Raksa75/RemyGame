using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HorizontalMovementController : MonoBehaviour
{
    [Header("Paramètres de déplacement")]
    [Tooltip("Vitesse de déplacement sur l'axe X")]
    public float speed = 5f;
    [Tooltip("Force du saut")]
    public float jumpForce = 7f;
    [Tooltip("Gravité appliquée")]
    public float gravity = 15f;
    [Tooltip("LayerMask pour détecter les obstacles")]
    public LayerMask environmentMask;
    [Tooltip("Distance tampon pour les murs")]
    public float wallCheckBuffer = 0.05f;

    private CharacterController cc;
    private float blockedDir = 0f;
    private float verticalVelocity = 0f;
    private bool isGrounded;

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
        CheckGrounded();
        HandleMovement();
    }

    void HandleMovement()
    {
        // 1️⃣ Gestion du déplacement horizontal
        float h = Input.GetAxis("Horizontal");
        float sign = Mathf.Sign(h);

        if (Mathf.Abs(h) > 0.01f)
        {
            if (blockedDir == 0f || sign != blockedDir)
            {
                Vector3 direction = new Vector3(h, 0f, 0f).normalized;
                float moveDist = speed * Time.deltaTime;

                Vector3 origin = transform.TransformPoint(cc.center);
                if (!Physics.Raycast(origin, direction, moveDist + wallCheckBuffer, environmentMask))
                {
                    blockedDir = 0f;
                    cc.Move(direction * moveDist);
                }
                else
                {
                    blockedDir = sign;
                }
            }
        }

        // 2️⃣ Gestion du saut et de la gravité
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        verticalVelocity -= gravity * Time.deltaTime; // Gravité appliquée
        cc.Move(new Vector3(0f, verticalVelocity * Time.deltaTime, 0f)); // Appliquer gravité et saut
    }

    void CheckGrounded()
    {
        isGrounded = cc.isGrounded;

        // Si au sol, reset la vélocité verticale
        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1f; // Léger push vers le bas pour éviter flottement
        }
    }

    void Jump()
    {
        verticalVelocity = Mathf.Sqrt(2f * jumpForce * gravity); // Calcul basé sur l'énergie potentielle
        Debug.Log($"[HorizontalMovement] Saut déclenché avec une force de {verticalVelocity} !");
    }

    void OnDestroy()
    {
        CharacterManager.Instance?.RemoveCharacter(gameObject);
    }
}
