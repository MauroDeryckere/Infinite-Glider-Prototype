using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GlideController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float turnSpeed = 60f;
    [SerializeField] private float glideLift = 5f;
    [SerializeField] private float gravityForce = 9.8f;

    [Header("Glide Control")]
    [SerializeField] private float liftDecay = 0.98f;  // how quickly lift fades when not gliding
    [SerializeField] private float minAltitude = -10f; // respawn threshold

    [Header("Debug")]
    [SerializeField] private bool isGliding = true;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // handled manually
        rb.useGravity = false; 
    }

    private void Update()
    {
        HandleInput();
        CheckRespawn();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void HandleInput()
    {
        // Toggle glide state (for later shield or dive controls)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isGliding = !isGliding;
        }
    }

    private void ApplyMovement()
    {
        // Forward motion
        Vector3 forwardMove = transform.forward * forwardSpeed;

        // Gravity and lift
        Vector3 verticalForce = Vector3.down * gravityForce;

        if (isGliding)
        {
            verticalForce += Vector3.up * glideLift;
            glideLift *= liftDecay; // smooth decay over time
        }

        // Apply rotation (left/right)
        float turnInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, turnInput * turnSpeed * Time.fixedDeltaTime);

        // Final velocity
        rb.linearVelocity = forwardMove + verticalForce;
    }

    private void CheckRespawn()
    {
        if (transform.position.y < minAltitude)
        {
            transform.position = Vector3.up * 5f;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
