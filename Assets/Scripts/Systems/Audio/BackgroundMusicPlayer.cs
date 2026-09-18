using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    private const string BgmResourcePath = "Music/BGM";

    private static BackgroundMusicPlayer instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null)
        {
            instance.EnsurePlaying();
            return;
        }

        GameObject musicRoot = new("BackgroundMusicPlayer");
        instance = musicRoot.AddComponent<BackgroundMusicPlayer>();
        DontDestroyOnLoad(musicRoot);
    }

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;

        if (audioSource.clip == null)
        {
            audioSource.clip = Resources.Load<AudioClip>(BgmResourcePath);
        }

        EnsurePlaying();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void EnsurePlaying()
    {
        if (audioSource == null || audioSource.clip == null || audioSource.isPlaying)
        {
            return;
        }

        audioSource.Play();
    }
}
