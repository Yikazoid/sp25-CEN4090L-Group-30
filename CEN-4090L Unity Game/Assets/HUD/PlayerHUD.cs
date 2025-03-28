using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Image[] healthIcons;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public int maxHealth = 5;
    public int currentHealth = 5;

    public Text partsText;
    private int partsCollected = 0;

    void Start()
    {
        UpdateHealthDisplay();
        UpdatePartsDisplay();
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthDisplay();
    }

    void UpdateHealthDisplay()
    {
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (i < currentHealth)
                healthIcons[i].sprite = fullHeart;
            else
                healthIcons[i].sprite = emptyHeart;
        }
    }

    public void AddPart()
    {
        partsCollected++;
        UpdatePartsDisplay();
    }

    void UpdatePartsDisplay()
    {
        partsText.text = "Parts: " + partsCollected.ToString();
    }
}
