using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private DayNightCycle dayNightCycle;

    [Header("Stat Bars")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider hungerSlider;

    [Header("Inventory Text")]
    [SerializeField] private TMP_Text mushroomText;
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text rockText;

    [Header("Time and Status")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text statusText;

    [Header("Panels")]
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private GameObject statusPanel;

    private void Start()
    {
        if (playerStats == null || inventory == null ||
            dayNightCycle == null)
        {
            Debug.LogError("PlayerHUD: Assign PlayerStats, InventoryManager, and DayNightCycle.");
            enabled = false;
            return;
        }

        SetSliderRange(healthSlider);
        SetSliderRange(staminaSlider);
        SetSliderRange(hungerSlider);
    }

    private void Update()
    {
        UpdateStats();
        UpdateInventory();
        UpdateTime();
        UpdateStatus();
    }

    private void SetSliderRange(Slider slider)
    {
        if (slider == null)
            return;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.interactable = false;
    }

    private void UpdateStats()
    {
        if (healthSlider != null)
            healthSlider.value = playerStats.maxHealth > 0f
                ? playerStats.currentHealth / playerStats.maxHealth : 0f;

        if (staminaSlider != null)
            staminaSlider.value = playerStats.maxStamina > 0f
                ? playerStats.currentStamina / playerStats.maxStamina : 0f;

        if (hungerSlider != null)
            hungerSlider.value = playerStats.maxHunger > 0f
                ? playerStats.currentHunger / playerStats.maxHunger : 0f;
    }

    private void UpdateInventory()
    {
        if (mushroomText != null)
            mushroomText.text = "Mushrooms: " + inventory.mushrooms;

        if (woodText != null)
            woodText.text = "Wood: " + inventory.wood;

        if (rockText != null)
            rockText.text = "Small Rocks: " + inventory.smallRocks;
    }

    private void UpdateTime()
    {
        if (timeText == null)
            return;

        float time = dayNightCycle.CurrentTime;
        int hours = Mathf.FloorToInt(time);
        int minutes = Mathf.FloorToInt((time - hours) * 60f);

        string period = hours >= 12 ? "PM" : "AM";
        int displayHour = hours % 12;

        if (displayHour == 0)
            displayHour = 12;

        timeText.text = displayHour.ToString("00") + ":" +
                        minutes.ToString("00") + " " + period;
    }

    private void UpdateStatus()
    {
        bool showWarning = false;
        string message = "";

        if (playerStats.IsDead)
        {
            message = "YOU DIED";
            showWarning = true;
        }
        else if (playerStats.IsHungry)
        {
            message = "You are hungry! Find food.";
            showWarning = true;
        }
        else if (playerStats.currentStamina <= 10f)
        {
            message = "Exhausted! Rest to recover stamina.";
            showWarning = true;
        }

        if (statusText != null)
            statusText.text = message;

        if (statusPanel != null)
            statusPanel.SetActive(showWarning);
    }
}
