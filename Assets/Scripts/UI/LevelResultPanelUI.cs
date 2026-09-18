using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelResultPanelUI : MonoBehaviour
{
    public GameObject Root;
    public TMP_Text TitleText;
    public TMP_Text SummaryText;
    public TMP_Text StatsText;
    public Button RetryButton;
    public Button ContinueButton;

    private void Start()
    {
        Hide();
    }

    public void Show(LevelSummary summary, string primaryLabel, string secondaryLabel, UnityAction retryAction, UnityAction continueAction)
    {
        if (Root != null)
        {
            Root.SetActive(true);
        }

        if (TitleText != null)
        {
            TitleText.SetText(summary.Title);
        }

        if (SummaryText != null)
        {
            SummaryText.SetText(summary.SummaryText);
        }

        if (StatsText != null)
        {
            StatsText.SetText($"{primaryLabel}: {summary.PrimaryScore}\n{secondaryLabel}: {summary.SecondaryScore}");
        }

        if (RetryButton != null)
        {
            RetryButton.onClick.RemoveAllListeners();
            RetryButton.onClick.AddListener(retryAction);
        }

        if (ContinueButton != null)
        {
            ContinueButton.onClick.RemoveAllListeners();
            ContinueButton.onClick.AddListener(continueAction);
        }
    }

    public void Hide()
    {
        if (Root != null)
        {
            Root.SetActive(false);
        }
    }
}
