using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class LevelControllerBase : MonoBehaviour
{
    public ObjectiveHUD HUD;
    public LevelResultPanelUI ResultPanel;

    protected bool IsCompleted { get; private set; }

    protected virtual void Start()
    {
        if (ResultPanel != null)
        {
            ResultPanel.Hide();
        }
    }

    protected void ShowHint(string message)
    {
        if (HUD != null)
        {
            HUD.SetHint(message);
        }

        if (GuideMessageUI.Instance != null)
        {
            GuideMessageUI.Instance.ShowMessage(message);
        }
    }

    protected void CompleteLevel(LevelSummary summary, string primaryLabel, string secondaryLabel, params NotebookEntryId[] entriesToUnlock)
    {
        if (IsCompleted)
        {
            return;
        }

        IsCompleted = true;

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.CompleteLevel(summary, entriesToUnlock);
        }

        if (Player.Instance != null)
        {
            Player.Instance.PlayerMovement.IsInteracting = true;
        }

        ReturnToNotebook();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToNotebook()
    {
        if (GameProgressManager.Instance != null)
        {
            NotebookPageId targetPage = GameProgressManager.Instance.AreAllLevelsCompleted()
                ? NotebookPageId.FinalReview
                : NotebookPageId.LevelResult;

            GameProgressManager.Instance.RequestNotebookPage(targetPage);
        }

        SceneManager.LoadScene(GameSceneNames.Notebook);
    }
}
