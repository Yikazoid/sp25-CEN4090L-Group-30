using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damageAmount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var hud = FindObjectOfType<PlayerHUD>();
            if (hud != null)
                hud.TakeDamage(damageAmount);
        }
    }
}