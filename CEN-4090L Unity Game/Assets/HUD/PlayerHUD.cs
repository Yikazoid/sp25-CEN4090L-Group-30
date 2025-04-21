
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHUD : MonoBehaviour
{
    [Header("Health HUD")]
    public Image[] healthIcons;        // drag in your heart Images in order
    public Sprite fullHeart;           // your red‐heart sprite
    public Sprite emptyHeart;          // your grey/empty‐heart sprite
    public int    maxHealth = 5;       
    private   int currentHealth;

    [Header("Parts Counter")]
    public Text partsText;             // drag in a UI Text
    private int  partsCollected = 0;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
        UpdatePartsDisplay();
    }

    void UpdateHealthDisplay()
    {
        for (int i = 0; i < healthIcons.Length; i++)
        {
            healthIcons[i].sprite = (i < currentHealth) 
                ? fullHeart 
                : emptyHeart;
        }
    }

    /// <summary>
    
    /// </summary>
    public void TakeDamage(int amount = 1)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateHealthDisplay();

        if (currentHealth <= 0)
        {
            // simple game‑over: back to MainMenu
            SceneManager.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// </summary>
    public void AddPart()
    {
        partsCollected++;
        UpdatePartsDisplay();
    }

    void UpdatePartsDisplay()
    {
        partsText.text = "Parts: " + partsCollected;
    }
}
