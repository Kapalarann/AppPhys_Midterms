using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float rotationSpeed = 10f;
    public float jumpForce = 6f;

    [Header("References")]
    public Transform cameraTransform;

    private Rigidbody rb;
    bool isGrounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Recommended for character controllers
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    private void FixedUpdate()
    {
        Move();

        if(transform.position.y < -10f) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(horizontal, vertical).normalized;

        Vector3 moveDirection = Vector3.zero;

        if (input.sqrMagnitude > 0.01f)
        {
            // Camera-relative movement
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            moveDirection = (camForward * input.y + camRight * input.x).normalized;

            // Rotate toward movement direction
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        // Current horizontal velocity
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 horizontalVelocity =
            new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        // Desired velocity
        Vector3 targetVelocity = moveDirection * moveSpeed;

        // Choose accel or decel
        float accelRate =
            input.sqrMagnitude > 0.01f ? acceleration : deceleration;

        // Accelerate toward target velocity
        Vector3 velocityChange =
            Vector3.MoveTowards(
                horizontalVelocity,
                targetVelocity,
                accelRate * Time.fixedDeltaTime
            ) - horizontalVelocity;

        rb.AddForce(
            velocityChange,
            ForceMode.VelocityChange
        );
    }
    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        isGrounded = false;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}