using UnityEngine;

public class NightObservationTarget : MonoBehaviour
{
    public NotebookEntryId EntryId;
    public InsectData InsectData;
    public GameObject HighlightMarker;
    public GameObject ReadyMarker;
    public GameObject VisualRoot;
    public ParticleSystem PhotographEffect;
    public bool EnableFloatMotion;
    public Vector3 MotionAmplitude = new(0.15f, 0.1f, 0f);
    public float MotionSpeed = 1f;

    private bool isPhotographed;
    private Vector3 startLocalPosition;

    public bool IsPhotographed => isPhotographed;

    private void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (isPhotographed || !EnableFloatMotion)
        {
            return;
        }

        float t = Time.time * MotionSpeed;
        transform.localPosition = startLocalPosition + new Vector3(
            Mathf.Sin(t) * MotionAmplitude.x,
            Mathf.Cos(t * 1.2f) * MotionAmplitude.y,
            Mathf.Sin(t * 0.6f) * MotionAmplitude.z);
    }

    public void SetFocused(bool isFocused, bool isReadyToPhotograph)
    {
        if (HighlightMarker != null)
        {
            HighlightMarker.SetActive(!isPhotographed && isFocused);
        }

        if (ReadyMarker != null)
        {
            ReadyMarker.SetActive(!isPhotographed && isReadyToPhotograph);
        }
    }

    public void Photograph()
    {
        if (isPhotographed)
        {
            return;
        }

        isPhotographed = true;
        SetFocused(false, false);

        if (PhotographEffect != null)
        {
            PhotographEffect.transform.SetParent(null, true);
            PhotographEffect.Play();
            Destroy(PhotographEffect.gameObject, PhotographEffect.main.duration + PhotographEffect.main.startLifetime.constantMax + 0.2f);
        }

        if (VisualRoot != null)
        {
            VisualRoot.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
