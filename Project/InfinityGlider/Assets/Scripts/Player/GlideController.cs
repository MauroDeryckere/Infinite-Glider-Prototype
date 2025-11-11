using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class GlideController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float turnSpeed = 60f;
    [SerializeField] private float glideLift = 5f;
    [SerializeField] private float gravityForce = 9.8f;

    [Header("Glide Control")]
    [SerializeField] private float liftDecay = 0.98f;
    [SerializeField] private float minAltitude = -10f;

    [Header("Debug")]
    [SerializeField] private bool isGliding = true;

    private Rigidbody rb;
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool glideHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // handled manually
        rb.useGravity = false;

        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Player.Glide.performed += _ => glideHeld = true;
        controls.Player.Glide.canceled += _ => glideHeld = false;
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }


    private void Update()
    {
        CheckRespawn();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        // Base Forward motion
        Vector3 forwardMove = transform.forward * forwardSpeed;

        // Gravity and lift
        float turnInput = moveInput.x;
        transform.Rotate(Vector3.up, turnInput * turnSpeed * Time.fixedDeltaTime);

        Vector3 verticalForce = Vector3.down * gravityForce;

        if (isGliding || glideHeld)
        {
            verticalForce += Vector3.up * glideLift;
            glideLift *= liftDecay;
        }

        rb.linearVelocity = forwardMove + verticalForce;
    }

    private void CheckRespawn()
    {
        if (transform.position.y < minAltitude)
        {
            transform.position = new Vector3(0f, 5f, transform.position.z);
            rb.linearVelocity = Vector3.zero;
        }
    }
}
