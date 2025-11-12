using UnityEngine;

public class ShieldPowerUp : PowerUp
{
    [SerializeField] private float duration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out GlideController glide))
        {
            glide.ActivateShield(duration);

            base.PlayPickupSound();

            Destroy(gameObject);
        }
    }
}
