using UnityEngine;

public class UpliftPowerUp : PowerUp
{
    [SerializeField] private float boostForce = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out GlideController glide))
        {
            glide.ApplyUpliftBoost(boostForce);
            Destroy(gameObject);
        }
    }
}
