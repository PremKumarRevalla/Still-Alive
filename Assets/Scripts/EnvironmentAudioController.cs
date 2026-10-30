using UnityEngine;

public class EnvironmentAudioController : MonoBehaviour
{
    [Header("Volume")]
    [Range(0f, 1f)]
    public float ambientVolume = 0.2f;

    [Range(0f, 1f)]
    public float rainVolume = 0.3f;

    [Range(0f, 1f)]
    public float fireVolume = 0.4f;

    [Header("References")]
    [SerializeField] private RainWeatherController rainController;

    private AudioSource audioSource;
    private AudioClip generatedClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;

        generatedClip = CreateNoiseClip();

        audioSource.clip = generatedClip;
        audioSource.volume = ambientVolume;
        audioSource.Play();
    }

    private AudioClip CreateNoiseClip()
    {
        int sampleRate = 44100;
        int sampleCount = sampleRate * 5;

        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            samples[i] = Random.Range(-1f, 1f) * 0.15f;
        }

        AudioClip clip = AudioClip.Create(
            "GeneratedEnvironmentSound",
            sampleCount,
            1,
            sampleRate,
            false
        );

        clip.SetData(samples, 0);

        return clip;
    }

    private void Update()
    {
        if (rainController == null)
            return;

        float targetVolume = rainController.IsRaining
            ? rainVolume
            : ambientVolume;

        audioSource.volume = Mathf.Lerp(
            audioSource.volume,
            targetVolume,
            Time.deltaTime * 2f
        );
    }
}
