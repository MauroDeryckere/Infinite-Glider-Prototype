using UnityEngine;
using Unity.Cinemachine;

public class ObstacleProximityShake : MonoBehaviour
{
    [SerializeField] private float triggerDistance = 20f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float maxShakeForce = 1.5f;
    [SerializeField] private float shakeCooldown = 0.5f;

    [SerializeField] private UIDangerIndicator dangerIndicator;


    private CinemachineImpulseSource impulseSource;
    private float lastShakeTime;

    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (impulseSource == null)
        {
            Debug.LogError("No CinemachineImpulseSource found on this GameObject.");
        }
    }


    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver())
        {
            return;
        }

        // A separate larger trigger box / collider per obstacle would be more optimal
        Collider[] obstacles = Physics.OverlapSphere(transform.position, triggerDistance, obstacleLayer);
        if (obstacles.Length > 0 && Time.time - lastShakeTime > shakeCooldown)
        {
            float nearest = float.MaxValue;
            foreach (var obstacle in obstacles)
            {
                float dist = Vector3.Distance(transform.position, obstacle.transform.position);
                if (dist < nearest) nearest = dist;
            }

            float intensity = Mathf.Clamp01(1f - (nearest / triggerDistance));
            float finalForce = Mathf.Lerp(0.2f, maxShakeForce, intensity);

            dangerIndicator?.Flash(intensity);

            impulseSource.GenerateImpulseWithForce(finalForce);
            lastShakeTime = Time.time;
        }
    }
}
