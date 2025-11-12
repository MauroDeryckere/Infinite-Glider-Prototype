using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float volume = 0.8f;

    protected void PlayPickupSound()
    {
        if (pickupSound)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }
    }
}
