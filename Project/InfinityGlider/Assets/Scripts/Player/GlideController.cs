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
    [SerializeField] private GameObject glider;

    [Header("Shield Settings")]
    [SerializeField] private GameObject shieldVisual;
    [SerializeField] private float shieldFadeSpeed = 2f;

    private float shieldTimer = 0f;
    private float baseShieldAlpha = 1f; // remember original alpha

    [Header("Debug")]
    [SerializeField] private bool isGliding = true;
    [SerializeField] private bool hasShield = false;

    private Rigidbody rb;
    private PlayerControls controls;
    private Vector2 moveInput;

    [Header("Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float deathSoundVolume = 0.8f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // handled manually
        rb.useGravity = false;

        controls = new PlayerControls();

        baseRotation = transform.rotation; // remember starting forward direction

        if (shieldVisual)
        {
            var renderer = shieldVisual.GetComponent<Renderer>();
            if (renderer && renderer.sharedMaterial.HasProperty("_Color"))
            {
                // store starting alpha
                baseShieldAlpha = renderer.sharedMaterial.color.a;
            }
            shieldVisual.SetActive(false);
        }
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Player.Glide.performed += _ =>
        {
            isGliding = false;
            glider?.SetActive(false);
        };

        controls.Player.Glide.canceled += _ =>
        {
            isGliding = true;
            glider?.SetActive(true);
        };
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void Update()
    {
        UpdateShield();
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

        Vector3 forwardMove = new Vector3();
        // Gravity and Glide Lift
        Vector3 verticalForce = Vector3.down * gravityForce;

        if (isGliding)
        {
            verticalForce += Vector3.up * glideLift;
            glideLift *= liftDecay;

            // Base Forward motion
            forwardMove = transform.forward * forwardSpeed;
        }
        else
        {
            verticalForce = verticalForce * 3;
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
            if (shieldVisual)
            {
                shieldVisual.SetActive(false);
            }

            Debug.Log("Shield absorbed collision!");
            return;
        }

        Die();
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        // Disable input / physics
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        enabled = false;
        GameManager.Instance?.OnPlayerDied();

        if (deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, deathSoundVolume);
        }
    }


    public void ActivateShield(float duration)
    {
        hasShield = true;
        shieldTimer = duration;

        Debug.Log($"Shield activated for {duration} seconds");

        if (shieldVisual)
        {
            shieldVisual.SetActive(true);
            var renderer = shieldVisual.GetComponent<Renderer>();

            if (renderer && renderer.material.HasProperty("_Color"))
            {
                Color c = renderer.material.color;
                c.a = baseShieldAlpha;
                renderer.material.color = c;
            }
        }
    }

    public void ApplyUpliftBoost(float force)
    {
        Vector3 boostDirection = (Vector3.up * 0.8f + transform.forward * 0.2f).normalized;

        rb.AddForce(boostDirection * force, ForceMode.VelocityChange);
        Debug.Log($"Uplift boost impulse applied (dir: {boostDirection}, force: {force})");
    }

    private void UpdateShield()
    {
        if (!hasShield)
        {
            return;
        }

        shieldTimer -= Time.deltaTime;
        if (shieldTimer <= 0f)
        {
            hasShield = false;
            if (shieldVisual)
            {
                shieldVisual.SetActive(false);
            }

            Debug.Log("Shield expired!");
        }
        else if (shieldVisual)
        {
            if (shieldTimer < 1f)
            {
                var renderer = shieldVisual.GetComponent<Renderer>();
                if (renderer && renderer.material.HasProperty("_Color"))
                {
                    Color c = renderer.material.color;
                    c.a = baseShieldAlpha * Mathf.PingPong(Time.time * shieldFadeSpeed, 1f);
                    renderer.material.color = c;
                }
            }
        }
    }
}
