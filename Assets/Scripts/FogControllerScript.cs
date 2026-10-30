using UnityEngine;

public class FogControllerScript : MonoBehaviour
{
    [Header("Fog Settings")]
    [SerializeField] private float maximumFogDensity = 0.015f;
    [SerializeField] private float minimumFogDensity = 0.0005f;

    [Header("Morning Fog Timing")]
    [SerializeField] private float fogStartHour = 4f;
    [SerializeField] private float fogPeakHour = 5f;
    [SerializeField] private float fogFadeStartHour = 6f;
    [SerializeField] private float fogEndHour = 10f;

    [Header("Transition")]
    [SerializeField] private float transitionSpeed = 0.5f;

    private float currentHour;
    private float targetDensity;
    private bool receivedTime;

    private void Start()
    {
        RenderSettings.fog = true;
        targetDensity = RenderSettings.fogDensity;
    }

    // Call this from your existing day/night cycle.
    public void SetTimeOfDay(float hour)
    {
        currentHour = Mathf.Repeat(hour, 24f);
        receivedTime = true;
    }

    private void Update()
    {
        if (!receivedTime)
            return;

        targetDensity = CalculateFogDensity(currentHour);

        RenderSettings.fogDensity = targetDensity;
    }

    private float CalculateFogDensity(float hour)
    {
        // Dense fog during the early morning.
        if (hour >= fogStartHour && hour < fogPeakHour)
        {
            float t = Mathf.InverseLerp(
                fogStartHour, fogPeakHour, hour);

            return Mathf.Lerp(
                minimumFogDensity, maximumFogDensity, t);
        }

        if (hour >= fogPeakHour && hour < fogFadeStartHour)
        {
            return maximumFogDensity;
        }

        // Gradually clear the fog after 6 AM.
        if (hour >= fogFadeStartHour && hour < fogEndHour)
        {
            float t = Mathf.InverseLerp(
                fogFadeStartHour, fogEndHour, hour);

            return Mathf.Lerp(
                maximumFogDensity, 0f, t);
        }

        // No morning fog during the rest of the day/night.
        return 0f;
    }
}