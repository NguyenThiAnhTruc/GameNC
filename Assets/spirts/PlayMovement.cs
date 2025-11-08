using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform groundCheck;

    [Header("Movement Settings")]
    public float speed = 12f;
    public float jumpHeight = 3f;

    [Header("Gravity / Ground Settings")]
    public float gravity = -19.62f;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;

    void Awake()
    {
        if (!controller) controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!controller || !groundCheck) return;

        // Kiểm tra chạm đất
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Nhận input di chuyển
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z).normalized;
        controller.Move(move * speed * Time.deltaTime);

        // Nhảy
        if (isGrounded && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space)))
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Cập nhật trọng lực
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Nhận trigger để debug vùng nhặt đồ
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player chạm trigger của: " + other.name);
    }

    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = isGrounded ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
