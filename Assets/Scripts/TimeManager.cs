using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Skybox Materials")]
    [SerializeField] private Material skyboxNight;
    [SerializeField] private Material skyboxSunrise;
    [SerializeField] private Material skyboxDay;
    [SerializeField] private Material skyboxSunset;

    [Header("Light Transition Gradients")]
    [SerializeField] private Gradient graddientNightToSunrise;
    [SerializeField] private Gradient graddientSunriseToDay;
    [SerializeField] private Gradient graddientDayToSunset;
    [SerializeField] private Gradient graddientSunsetToNight;

    [Header("Global Light")]
    [SerializeField] private Light globalLight;

    private int minutes;
    public int Minutes
    { get { return minutes; } set { minutes = value; OnMinutesChange(value); } }

    private int hours = 5;
    public int Hours
    { get { return hours; } set { hours = value; OnHoursChange(value); } }

    private int days;
    public int Days
    { get { return days; } set { days = value; } }

    private float tempSecond;

    // Speed factor: 15 seconds for 24 hours
    private float realTimeDayDuration = 30f;
    private float secondsPerInGameMinute;

    void Start()
    {
        secondsPerInGameMinute = realTimeDayDuration / (24 * 60); // Calculate speed
        OnHoursChange(hours); // Ensure skybox updates at start
    }

    void Update()
    {
        tempSecond += Time.deltaTime;
        if (tempSecond >= secondsPerInGameMinute)
        {
            Minutes += 1;
            tempSecond = 0;
        }
    }

    private void OnMinutesChange(int value)
    {
        globalLight.transform.Rotate(Vector3.right, (360f / 1440f), Space.World);

        if (value >= 60)
        {
            Hours++;
            minutes = 0;
            OnHoursChange(Hours);
        }
        if (Hours >= 24)
        {
            Hours = 0;
            Days++;
        }
    }

    private void OnHoursChange(int value)
    {
        if (value == 6)
        {
            StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, secondsPerInGameMinute * 120));
            StartCoroutine(LerpLight(graddientNightToSunrise, secondsPerInGameMinute * 120));
        }
        else if (value == 8)
        {
            StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, secondsPerInGameMinute * 120));
            StartCoroutine(LerpLight(graddientSunriseToDay, secondsPerInGameMinute * 120));
        }
        else if (value == 18)
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, secondsPerInGameMinute * 120));
            StartCoroutine(LerpLight(graddientDayToSunset, secondsPerInGameMinute * 120));
        }
        else if (value == 22)
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, secondsPerInGameMinute * 120));
            StartCoroutine(LerpLight(graddientSunsetToNight, secondsPerInGameMinute * 120));
        }
    }

    private IEnumerator LerpSkybox(Material start, Material end, float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            RenderSettings.skybox.Lerp(start, end, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        RenderSettings.skybox = end;
    }

    private IEnumerator LerpLight(Gradient lightGradient, float time)
    {
        float startIntensity = globalLight.intensity;
        float targetIntensity = (lightGradient == graddientNightToSunrise || lightGradient == graddientSunriseToDay) ? 1.2f : 0.4f;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            float t = elapsedTime / time;
            globalLight.color = lightGradient.Evaluate(t);
            RenderSettings.fogColor = globalLight.color;
            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            RenderSettings.fogColor = globalLight.color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        globalLight.intensity = targetIntensity;
    }
}
