using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var glide = other.GetComponent<GlideController>();
        if (glide != null)
        {
            glide.Die();
        }
    }
}