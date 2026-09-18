using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    public AudioClip FootstepSound;
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = FootstepSound;
        audioSource.volume = 0f;
    }

    public void PlayStepSound()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (!audioSource.isPlaying)
            audioSource.Play();

        fadeCoroutine = StartCoroutine(FadeIn(0.2f));
    }

    public void StopStepSound()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOut(0.3f));
    }

    private System.Collections.IEnumerator FadeIn(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 0.5f, t / duration);
            yield return null;
        }
        audioSource.volume = 0.5f;
    }

    private System.Collections.IEnumerator FadeOut(float duration)
    {
        float startVolume = audioSource.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();
    }
}
