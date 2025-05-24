using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    private const string MainSceneName = "MainScene";

    public void ToMainScene()
    {
        SceneManager.LoadScene(MainSceneName);
    }
}
