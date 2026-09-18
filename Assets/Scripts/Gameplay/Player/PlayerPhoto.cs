using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerPhoto : MonoBehaviour
{
    public Volume Volume;

    private AudioSource audioSource;
    private Bloom bloom;
    private bool isFlashing = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (Volume != null && Volume.profile != null)
        {
            Volume.profile.TryGet(out bloom);
        }
    }

    private void Update()
    {
        CapturePhoto();
    }

    private void CapturePhoto()
    {
        if (InputManager.Instance != null && InputManager.Instance.IsPhotoPressed)
        {
            if (!isFlashing)
            {
                if (audioSource != null)
                {
                    audioSource.Play();
                }

                StartCoroutine(FlashCoroutine());
            }
        }
    }

    private IEnumerator FlashCoroutine()
    {
        isFlashing = true;
        float flashDurationA = 0.02f, flashDurationB = 0.08f, timer = 0f, maxIntensity = 30f;

        if (bloom == null)
        {
            yield return new WaitForSeconds(flashDurationA + flashDurationB);
            isFlashing = false;
            yield break;
        }

        bloom.intensity.value = 0f;

        while (timer < flashDurationA + flashDurationB)
        {
            if (timer < flashDurationA)
            {
                bloom.intensity.value = Mathf.Lerp(0f, maxIntensity, timer / flashDurationA);
            }
            else
            {
                bloom.intensity.value = Mathf.Lerp(maxIntensity, 0f, (timer - flashDurationA) / flashDurationB);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        
        bloom.intensity.value = 0f;
        isFlashing = false;
    }
}
