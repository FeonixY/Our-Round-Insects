using UnityEngine;

public class TimeManager : SingletonMonoBehaviour<TimeManager>
{
    public Light directionalLight;
    public float dayDuration = 60f;
    public Gradient lightColorGradient;
    public AnimationCurve lightIntensityCurve;
    public bool IsNightTime =>
        elapsedTime % dayDuration > dayDuration / 4f &&
        elapsedTime % dayDuration < dayDuration / 4f * 3f;

    private float elapsedTime = 0f;

    private void Update()
    {
        if (directionalLight == null)
            return;

        elapsedTime += Time.deltaTime;
        float timeOfDay = (elapsedTime % dayDuration) / dayDuration;

        float sunAngle = Mathf.Lerp(90f, -270f, timeOfDay);
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle, 0f, 0f));

        directionalLight.color = lightColorGradient.Evaluate(timeOfDay);
        directionalLight.intensity = lightIntensityCurve.Evaluate(timeOfDay);
    }
}
