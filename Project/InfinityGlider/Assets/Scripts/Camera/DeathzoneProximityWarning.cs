using UnityEngine;

public class DeathZoneProximityWarning : MonoBehaviour
{
    [SerializeField] private float warningDistance = 10f;
    [SerializeField] private LayerMask deathZoneLayer;
    [SerializeField] private UIDangerIndicator dangerIndicator;
    [SerializeField] private float cooldown = 0.5f;

    private float lastFlashTime = 0f;

    void Update()
    {
        if (Time.time - lastFlashTime < cooldown)
        {
            return;
        }

        Collider[] zones = Physics.OverlapSphere(transform.position, warningDistance, deathZoneLayer);
        if (zones.Length > 0)
        {
            float nearest = float.MaxValue;
            foreach (var zone in zones)
            {
                float dist = Vector3.Distance(transform.position, zone.transform.position);
                if (dist < nearest) nearest = dist;
            }

            float intensity = Mathf.Clamp01(1f - (nearest / warningDistance));
            dangerIndicator?.Flash(intensity);
            lastFlashTime = Time.time;       
        }
    }
}
