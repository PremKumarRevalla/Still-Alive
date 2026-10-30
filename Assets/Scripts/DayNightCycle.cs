using UnityEngine;
using UnityEngine.Rendering;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private Light sun;
    [SerializeField] private float dayLengthMinutes = 10f;

    [Range(0f, 24f)]
    [SerializeField] private float currentTime = 12f;

    [Header("Sunlight")]
    [SerializeField] private float maxSunIntensity = 1.2f;
    [SerializeField] private Color dayAmbient = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private Color nightAmbient = new Color(0.08f, 0.12f, 0.25f);

    [Header("Morning Fog")]
    [SerializeField] private FogControllerScript morningFog;

    [Header("Night Settings")]
    [SerializeField] private float nightMoonIntensity = 0.025f;
    [SerializeField] private Color nightSunColor = new Color(0.35f, 0.42f, 0.65f);
    [SerializeField] private float nightReflectionIntensity = 0.15f;

    [Header("Rainy Weather Lighting")]
    [SerializeField] private float rainySunMultiplier = 0.65f;
    [SerializeField] private float rainyAmbientMultiplier = 0.8f;

    [Header("Rainy Sky")]
    [SerializeField] private Color clearSkyTint = Color.white;
    [SerializeField] private Color rainySkyTint = new Color(0.55f, 0.60f, 0.68f);
    [SerializeField] private float rainySkyExposure = 0.7f;

    private Material skyboxMaterial;
    private Color originalSkyTint;
    private float originalSkyExposure;

    private float weatherBlend;
    private float weatherTarget;

    public float CurrentTime => currentTime;
    public bool IsNight => currentTime < 6f || currentTime >= 18f;

    private void Start()
    {
        // Force ambient lighting to use our configured colours.
        RenderSettings.ambientMode = AmbientMode.Flat;

        // Create an instance of the current skybox material.
        if (RenderSettings.skybox != null)
        {
            skyboxMaterial = new Material(RenderSettings.skybox);
            RenderSettings.skybox = skyboxMaterial;

            if (skyboxMaterial.HasProperty("_Tint"))
                originalSkyTint = skyboxMaterial.GetColor("_Tint");

            if (skyboxMaterial.HasProperty("_Exposure"))
                originalSkyExposure = skyboxMaterial.GetFloat("_Exposure");
        }
    }

    private void Update()
    {
        // Advance the clock.
        currentTime += Time.deltaTime / (dayLengthMinutes * 60f) * 24f;

        if (currentTime >= 24f)
            currentTime -= 24f;
        weatherBlend = Mathf.MoveTowards(weatherBlend,weatherTarget,Time.deltaTime * 0.25f);

        UpdateSun();
        UpdateRainySky();

        // Pass the existing game time to the fog controller.
        if (morningFog != null)
        {
            morningFog.SetTimeOfDay(currentTime);
        }
    }
    public void SetRainyWeather(bool raining)
    {
        weatherTarget = raining ? 1f : 0f;
    }


    private void UpdateSun()
    {
        if (sun == null)
            return;

        // Sun rotation
        float sunAngle = currentTime / 24f * 360f - 90f;

        sun.transform.rotation = Quaternion.Euler(
            sunAngle,
            170f,
            0f
        );

        // Calculate daylight
        float daylight = Mathf.Clamp01(
            Mathf.Sin((currentTime - 6f) / 12f * Mathf.PI)
        );

        // Smooth transition between day and night
        daylight = Mathf.SmoothStep(0f, 1f, daylight);

        // Rainy weather blend (0 = clear, 1 = rainy)
        float rainySun = Mathf.Lerp(
            1f,
            rainySunMultiplier,
            weatherBlend
        );

        float rainyAmbient = Mathf.Lerp(
            1f,
            rainyAmbientMultiplier,
            weatherBlend
        );

        // Sun / moon intensity
        float normalIntensity = Mathf.Lerp(
            nightMoonIntensity,
            maxSunIntensity,
            daylight
        );

        sun.intensity = normalIntensity * rainySun;

        // Lighting colour
        sun.color = Color.Lerp(
            nightSunColor,
            Color.white,
            daylight
        );

        // Ambient lighting
        Color normalAmbient = Color.Lerp(
            nightAmbient,
            dayAmbient,
            daylight
        );

        RenderSettings.ambientLight = normalAmbient * rainyAmbient;

        // Environmental reflections
        float normalReflection = Mathf.Lerp(
            nightReflectionIntensity,
            1f,
            daylight
        );

        RenderSettings.reflectionIntensity = normalReflection * Mathf.Lerp(1f, 0.8f, weatherBlend);
    }
    private void UpdateRainySky()
    {
        if (skyboxMaterial == null)
            return;

        Color targetTint = Color.Lerp(
            originalSkyTint,
            rainySkyTint,
            weatherBlend
        );

        if (skyboxMaterial.HasProperty("_Tint"))
        {
            skyboxMaterial.SetColor("_Tint", targetTint);
        }

        if (skyboxMaterial.HasProperty("_Exposure"))
        {
            float targetExposure = Mathf.Lerp(
                originalSkyExposure,
                rainySkyExposure,
                weatherBlend
            );

            skyboxMaterial.SetFloat("_Exposure", targetExposure);
        }

        DynamicGI.UpdateEnvironment();
    }

}