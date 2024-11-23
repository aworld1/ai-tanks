// HealthBar.cs
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFillImage;
    private Player player;

    void Start()
    {

        player = GetComponentInParent<Player>();
        if (player != null)
        {
            // Subscribe to the OnHealthChanged event
            player.OnHealthChanged += UpdateHealthBar;

            // Initialize the health bar
            UpdateHealthBar();
        }
        else
        {
            Debug.LogWarning("HealthBar: No Player component found in parent.");
        }
    }

    void LateUpdate()
    {
        // Reset rotation to keep the health bar upright
        transform.rotation = Quaternion.identity;
        transform.position = player.transform.position + new Vector3(0, -0.7f, 0);

    }

    void OnDestroy()
    {
        if (player != null)
        {
            // Unsubscribe from the event to prevent memory leaks
            player.OnHealthChanged -= UpdateHealthBar;
        }
    }

    public void UpdateHealthBar()
    {
        if (player != null && healthFillImage != null)
        {
            float healthPercentage = Mathf.Clamp01((float)player.currentHealth / player.maxHealth);
            healthFillImage.fillAmount = healthPercentage;

            // Change color based on health percentage
            if (healthPercentage > 0.5f)
            {
                healthFillImage.color = Color.green;
            }
            else if (healthPercentage > 0.2f)
            {
                healthFillImage.color = Color.yellow;
            }
            else
            {
                healthFillImage.color = Color.red;
            }
        }
    }
}
