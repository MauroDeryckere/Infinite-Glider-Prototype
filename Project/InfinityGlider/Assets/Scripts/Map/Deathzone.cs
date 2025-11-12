using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var glide = other.GetComponent<GlideController>();
        if (glide != null)
        {
            Debug.Log("Player entered death zone");
            glide.Die();
        }
    }
}