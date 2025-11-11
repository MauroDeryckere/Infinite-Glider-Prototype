using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class GlideController : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private LayerMask obstacleMask;
    private bool isDead = false;

    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float turnSpeed = 60f;
    [SerializeField] private float glideLift = 5f;
    [SerializeField] private float gravityForce = 9.8f;
    [SerializeField] private float maxTurnAngle = 45f;

    private float currentYaw = 0f;
    private Quaternion baseRotation;

    [Header("Glide Control")]
    [SerializeField] private float liftDecay = 0.98f;

    [Header("Debug")]
    [SerializeField] private bool isGliding = true;
    [SerializeField] private bool hasShield = false;

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

        baseRotation = transform.rotation; // remember starting forward direction
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

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        //Turning
        float turnInput = moveInput.x;

        // Update yaw based on input and clamp to maxTurnAngle
        currentYaw += turnInput * turnSpeed * Time.fixedDeltaTime;
        currentYaw = Mathf.Clamp(currentYaw, -maxTurnAngle, maxTurnAngle);

        // Compute final rotation (keep within limited arc)
        Quaternion targetRotation = baseRotation * Quaternion.Euler(0f, currentYaw, 0f);

        float rollAngle = -currentYaw * 0.5f;
        targetRotation *= Quaternion.Euler(0f, 0f, rollAngle);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);

        // Base Forward motion
        Vector3 forwardMove = transform.forward * forwardSpeed;

        // Gravity and Glide Lift
        Vector3 verticalForce = Vector3.down * gravityForce;

        if (isGliding || glideHeld)
        {
            verticalForce += Vector3.up * glideLift;
            glideLift *= liftDecay;
        }

        // Combine forces
        rb.linearVelocity = forwardMove + verticalForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Hit: " + collision.gameObject.name + " on layer " + collision.gameObject.layer);

        if (isDead)
        {
            return; 
        }

        int layer = other.gameObject.layer;
        if (((1 << layer) & obstacleMask) == 0)
        {
            return;
        }

        if (hasShield)
        {
            hasShield = false;
            Debug.Log("Shield absorbed collision!");
            return;
        }

        Die();
    }

    private void Die()
    {
        isDead = true;

        // Disable input / physics
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        enabled = false;
        GameManager.Instance?.OnPlayerDied();
    }


    public void ActivateShield(float duration)
    {
        hasShield = true;
        //TODO
       // shieldTimer = duration;
        Debug.Log($"Shield activated for {duration} seconds");
    }

    public void ApplyUpliftBoost(float force)
    {
        rb.linearVelocity += Vector3.up * force;
        Debug.Log($"Uplift boost impulse applied: +{force}");
    }
}
