using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.ResetProgress();
            GameProgressManager.Instance.RequestNotebookPage(NotebookPageId.Intro);
        }

        SceneManager.LoadScene(GameSceneNames.Notebook);
    }

    public void OpenDictionary()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.RequestNotebookPage(NotebookPageId.LevelSelect, true);
        }

        SceneManager.LoadScene(GameSceneNames.Notebook);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
