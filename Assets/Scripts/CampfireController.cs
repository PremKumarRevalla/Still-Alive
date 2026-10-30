using UnityEngine;

public class CampfireController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private Light campfireLight;

    [Header("Rain Settings")]
    [SerializeField] private float rainExtinguishDelay = 5f;

    [Header("Lifetime Settings")]
    [SerializeField] private float fireLifetimeGameHours = 6f;
    [SerializeField] private float smokeLifetimeGameHours = 2f;

    private RainWeatherController rainController;
    private DayNightCycle dayNightCycle;

    private bool isExtinguished;
    private bool smokeFinished;

    private float rainTimer;
    private float lastGameTime;

    private float elapsedFireHours;
    private float elapsedSmokeHours;

    private void Start()
    {
        rainController = FindFirstObjectByType<RainWeatherController>();
        dayNightCycle = FindFirstObjectByType<DayNightCycle>();

        if (rainController == null || dayNightCycle == null)
        {
            Debug.LogError("RainWeatherController or DayNightCycle missing!");
            enabled = false;
            return;
        }

        lastGameTime = dayNightCycle.CurrentTime;

        // Start fire
        if (fireParticles != null)
            fireParticles.Play();

        if (campfireLight != null)
            campfireLight.enabled = true;

        // Keep smoke disabled initially
        if (smokeParticles != null)
        {
            smokeParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            smokeParticles.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateGameTime();
        UpdateRain();

        // Natural fire lifetime: extinguish after 6 game hours
        if (!isExtinguished &&
            elapsedFireHours >= fireLifetimeGameHours)
        {
            ExtinguishFire();
        }

        // Destroy after fire + smoke lifetime
        if (elapsedFireHours >=
            fireLifetimeGameHours + smokeLifetimeGameHours)
        {
            Debug.Log("Campfire completely expired!");
            Destroy(gameObject);
        }
    }

    private void UpdateGameTime()
    {
        float currentGameTime = dayNightCycle.CurrentTime;

        float difference = currentGameTime - lastGameTime;

        // Handle midnight transition
        if (difference < 0f)
            difference += 24f;

        elapsedFireHours += difference;

        // Count smoke lifetime after fire extinguishes
        if (isExtinguished && !smokeFinished)
        {
            elapsedSmokeHours += difference;

            if (elapsedSmokeHours >= smokeLifetimeGameHours)
            {
                DisableSmoke();
            }
        }

        lastGameTime = currentGameTime;
    }

    private void UpdateRain()
    {
        if (isExtinguished)
            return;

        if (rainController.IsRaining)
        {
            rainTimer += Time.deltaTime;

            if (rainTimer >= rainExtinguishDelay)
            {
                ExtinguishFire();
            }
        }
        else
        {
            rainTimer = 0f;
        }
    }

    private void ExtinguishFire()
    {
        if (isExtinguished)
            return;

        isExtinguished = true;

        // Turn off fire particles
        if (fireParticles != null)
        {
            fireParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }

        // Turn off fire light
        if (campfireLight != null)
            campfireLight.enabled = false;

        // Enable smoke
        if (smokeParticles != null)
        {
            smokeParticles.gameObject.SetActive(true);
            smokeParticles.Play();
        }

        Debug.Log("Fire extinguished! Smoke started.");
    }

    private void DisableSmoke()
    {
        smokeFinished = true;

        if (smokeParticles != null)
        {
            smokeParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            smokeParticles.gameObject.SetActive(false);
        }

        Debug.Log("Campfire smoke ended!");
    }
}