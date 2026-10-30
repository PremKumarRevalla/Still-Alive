using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaRecoveryRate = 5f;

    [Header("Stamina Consumption")]
    public float runningStaminaCost = 10f;
    public float jumpingStaminaCost = 15f;

    [Header("Hunger")]
    public float maxHunger = 100f;
    public float currentHunger = 100f;
    public float hungerDrainPerGameHour = 8f;
    public float hungryThreshold = 25f;
    public float starvationDamagePerGameHour = 10f;

    [Header("References")]
    [SerializeField] private DayNightCycle dayNightCycle;

    public bool IsHungry => currentHunger <= hungryThreshold;
    public bool IsDead => currentHealth <= 0f;

    private float lastGameTime;

    private void Start()
    {
        if (dayNightCycle == null)
            dayNightCycle = FindFirstObjectByType<DayNightCycle>();

        if (dayNightCycle == null)
        {
            Debug.LogError("DayNightCycle not found!");
            enabled = false;
            return;
        }

        lastGameTime = dayNightCycle.CurrentTime;
    }

    private void Update()
    {
        UpdateHunger();
        RecoverStamina();
    }
    public void RecoverStamina()
    {
        currentStamina = Mathf.Clamp(
            currentStamina + staminaRecoveryRate * Time.deltaTime,
            0f,
            maxStamina
        );
    }

    private void UpdateHunger()
    {
        float currentGameTime = dayNightCycle.CurrentTime;

        float gameHoursPassed = currentGameTime - lastGameTime;

        if (gameHoursPassed < 0f)
            gameHoursPassed += 24f;

        lastGameTime = currentGameTime;

        if (gameHoursPassed <= 0f || IsDead)
            return;

        // Calculate how many hours remain before hunger reaches zero
        float hoursUntilStarvation =
            currentHunger / hungerDrainPerGameHour;

        // Calculate time spent starving during this update
        float starvingHours = Mathf.Max(
            0f,
            gameHoursPassed - hoursUntilStarvation
        );

        // Reduce hunger
        currentHunger = Mathf.Max(
            0f,
            currentHunger - hungerDrainPerGameHour * gameHoursPassed
        );

        // Reduce health only after hunger reaches zero
        if (starvingHours > 0f)
        {
            TakeDamage(starvationDamagePerGameHour * starvingHours);
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead)
            return;

        currentHealth = Mathf.Clamp(
            currentHealth - amount, 0f, maxHealth
        );

        if (IsDead)
            Debug.Log("Player died!");
    }

    public void Heal(float amount)
    {
        if (IsDead)
            return;

        currentHealth = Mathf.Clamp(
            currentHealth + amount, 0f, maxHealth
        );
    }

    public void UseStamina(float amount)
    {
        currentStamina = Mathf.Clamp(
            currentStamina - amount, 0f, maxStamina
        );
    }

    public void RestoreStamina(float amount)
    {
        currentStamina = Mathf.Clamp(
            currentStamina + amount, 0f, maxStamina
        );
    }

    public void RestoreHunger(float amount)
    {
        currentHunger = Mathf.Clamp(
            currentHunger + amount, 0f, maxHunger
        );
    }
}
