using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerPhoto : MonoBehaviour
{
    public Volume Volume;

    private AudioSource audioSource;
    private Bloom bloom;
    private bool isFlashing = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Volume.profile.TryGet(out bloom);
    }

    private void Update()
    {
        CapturePhoto();
    }

    private void CapturePhoto()
    {
        if (InputManager.Instance.IsPhotoPressed)
        {
            if (!isFlashing)
            {
                audioSource.Play();
                StartCoroutine(FlashCoroutine());
                DetectPhotographableInView();
            }
        }
    }

    private void DetectPhotographableInView()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        if (!activeScene.isLoaded)
        {
            return;
        }

        var rootObjects = activeScene.GetRootGameObjects();
        foreach (var rootObject in rootObjects)
        {
            var photographables = rootObject.GetComponentsInChildren<Insect>();
            foreach (var photographable in photographables)
            {
                photographable.OnPhotographTaken();
            }
        }
    }

    private IEnumerator FlashCoroutine()
    {
        isFlashing = true;
        float flashDurationA = 0.02f, flashDurationB = 0.08f, timer = 0f, maxIntensity = 30f;
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
