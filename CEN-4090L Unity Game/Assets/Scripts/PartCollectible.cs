using UnityEngine;

public class PartCollectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHUD hud = FindObjectOfType<PlayerHUD>();
            if (hud != null)
            {
                hud.AddPart();
            }
            Destroy(gameObject);
        }
    }
}
