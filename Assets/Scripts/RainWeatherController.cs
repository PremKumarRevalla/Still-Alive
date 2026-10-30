using UnityEngine;

public class RainWeatherController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private ParticleSystem rainEffect;
    [SerializeField] private Transform player;
    public bool IsRaining { get; private set; }

    [Header("Random Weather")]
    [Range(0f, 1f)]
    [SerializeField] private float dailyRainChance = 0.35f;

    [SerializeField] private Vector2 rainStartTime = new Vector2(7f, 21f);
    [SerializeField] private Vector2 rainDuration = new Vector2(1f, 3f);

    [Header("Rain Settings")]
    [SerializeField] private float maximumEmission = 1500f;
    [SerializeField] private float transitionSpeed = 500f;
    [SerializeField] private Vector3 playerOffset = new Vector3(0f, 10f, 0f);

    private float previousTime;
    private float scheduledStart;
    private float scheduledEnd;

    private bool rainScheduled;
    private float currentEmission;

    private ParticleSystem.EmissionModule emission;

    private void Start()
    {
        if (dayNightCycle == null || rainEffect == null)
        {
            Debug.LogError("RainWeatherController references are missing!");
            enabled = false;
            return;
        }

        // Ensure the particle system GameObject is active.
        rainEffect.gameObject.SetActive(true);

        emission = rainEffect.emission;
        emission.rateOverTime = 0f;

        rainEffect.Stop();

        previousTime = dayNightCycle.CurrentTime;

        ScheduleRain();
    }

    private void Update()
    {
        float currentTime = dayNightCycle.CurrentTime;

        // Detect a new in-game day.
        if (currentTime < previousTime)
        {
            ScheduleRain();
        }

        previousTime = currentTime;

        bool shouldRain = rainScheduled &&
                          currentTime >= scheduledStart &&
                          currentTime < scheduledEnd;
        IsRaining = shouldRain;
        dayNightCycle.SetRainyWeather(shouldRain);

        UpdateRainEffect(shouldRain);
    }

    private void ScheduleRain()
    {
        rainScheduled = Random.value < dailyRainChance;

        if (!rainScheduled)
        {
            Debug.Log("Today's weather: No rain.");
            return;
        }

        float duration = Random.Range(
            rainDuration.x,
            rainDuration.y
        );

        float latestStart = Mathf.Min(
            rainStartTime.y,
            24f - duration
        );

        scheduledStart = Random.Range(
            rainStartTime.x,
            latestStart
        );

        scheduledEnd = scheduledStart + duration;

        Debug.Log(
            $"Rain scheduled at {scheduledStart:F1} for {duration:F1} game hours."
        );
    }

    private void UpdateRainEffect(bool shouldRain)
    {
        float targetEmission = shouldRain ? maximumEmission : 0f;

        if (shouldRain && !rainEffect.isPlaying)
        {
            rainEffect.Play();
        }

        currentEmission = Mathf.MoveTowards(
            currentEmission,
            targetEmission,
            transitionSpeed * Time.deltaTime
        );

        emission.rateOverTime = currentEmission;

        // Stop only after the rain has faded out.
        if (!shouldRain && currentEmission <= 0.01f &&
            rainEffect.isPlaying)
        {
            rainEffect.Stop();
        }
    }

    private void LateUpdate()
    {
        if (player != null && rainEffect != null)
        {
            rainEffect.transform.position =
                player.position + playerOffset;
        }
    }
}