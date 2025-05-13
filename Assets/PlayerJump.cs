using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Tooltip("Force du saut")]
    public float jumpForce = 7f;
    [Tooltip("Layer du sol")]
    public LayerMask groundLayer;
    [Tooltip("Délai avant stabilisation après un saut")]
    public float groundStabilizationDelay = 0.2f; // Ajoute un léger délai

    private Rigidbody rb;
    private bool isGrounded;
    private float lastJumpTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
    }

    void Update()
    {
        CheckGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, groundLayer))
        {
            if (rb.linearVelocity.y <= 0 && Time.time - lastJumpTime > groundStabilizationDelay)
            {
                isGrounded = true;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Stabilisation au sol après un délai
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    void Jump()
    {
        lastJumpTime = Time.time; // Enregistre le moment du saut
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Debug.Log("[PlayerJump] Saut déclenché !");
    }
}
