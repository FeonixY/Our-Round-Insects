using UnityEngine;

public class FlyingPestTarget : MonoBehaviour
{
    public GameObject LockMarker;
    public ParticleSystem CaptureEffect;
    public bool EnableHoverMotion = true;
    public Vector3 MotionAmplitude = new(0.4f, 0.25f, 0f);
    public float MotionSpeed = 1.2f;

    private bool isCaptured;
    private Vector3 startLocalPosition;

    public bool IsCaptured => isCaptured;

    private void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (isCaptured || !EnableHoverMotion)
        {
            return;
        }

        float t = Time.time * MotionSpeed;
        transform.localPosition = startLocalPosition + new Vector3(
            Mathf.Sin(t) * MotionAmplitude.x,
            Mathf.Cos(t * 1.4f) * MotionAmplitude.y,
            Mathf.Sin(t * 0.8f) * MotionAmplitude.z);
    }

    public void SetLocked(bool isLocked)
    {
        if (LockMarker != null)
        {
            LockMarker.SetActive(!isCaptured && isLocked);
        }
    }

    public void Capture()
    {
        if (isCaptured)
        {
            return;
        }

        isCaptured = true;
        SetLocked(false);

        if (CaptureEffect != null)
        {
            CaptureEffect.transform.SetParent(null, true);
            CaptureEffect.Play();
            Destroy(CaptureEffect.gameObject, CaptureEffect.main.duration + CaptureEffect.main.startLifetime.constantMax + 0.2f);
        }

        gameObject.SetActive(false);
    }
}
