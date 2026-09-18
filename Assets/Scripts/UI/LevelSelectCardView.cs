using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectCardView : MonoBehaviour
{
    private static readonly Color DefaultTitleColor = Color.white;
    private static readonly Color SelectedTitleColor = Color.white;
    private static readonly Color DefaultStatusColor = new(0.23f, 0.18f, 0.10f, 1f);
    private static readonly Color SelectedStatusColor = new(0.28f, 0.19f, 0.06f, 1f);

    public LevelId LevelId;
    public Button SelectButton;
    public Image PreviewImage;
    public Image SelectionFrame;
    public TMP_Text TitleText;
    public TMP_Text StatusText;
    public GameObject CompletedMarker;
    public Sprite PreviewSprite;

    [TextArea(2, 3)]
    public string Summary;

    [TextArea(1, 2)]
    public string Goal;

    private NotebookSceneManager owner;

    public bool IsCompleted => GameProgressManager.Instance != null && GameProgressManager.Instance.IsLevelCompleted(LevelId);

    public void Initialize(NotebookSceneManager manager)
    {
        owner = manager;

        if (SelectButton == null)
        {
            SelectButton = GetComponent<Button>();
        }

        if (SelectButton != null)
        {
            SelectButton.onClick.RemoveAllListeners();
            SelectButton.onClick.AddListener(OnSelected);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (TitleText != null)
        {
            TitleText.SetText(GetTitle());
        }

        if (StatusText != null)
        {
            StatusText.SetText(IsCompleted ? "Completed" : "Available");
        }

        if (CompletedMarker != null)
        {
            CompletedMarker.SetActive(IsCompleted);
        }

        if (PreviewImage != null && PreviewSprite != null)
        {
            PreviewImage.sprite = PreviewSprite;
            PreviewImage.preserveAspect = true;
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (SelectionFrame != null)
        {
            SelectionFrame.enabled = false;
        }

        transform.localScale = isSelected ? new Vector3(1.03f, 1.03f, 1f) : Vector3.one;

        if (TitleText != null)
        {
            TitleText.color = isSelected ? SelectedTitleColor : DefaultTitleColor;
        }

        if (StatusText != null)
        {
            StatusText.color = isSelected ? SelectedStatusColor : DefaultStatusColor;
        }
    }

    public string GetTitle()
    {
        return LevelId switch
        {
            LevelId.Flower => "Flower Habitat",
            LevelId.Grass => "Grass Habitat",
            LevelId.Night => "Night Habitat",
            _ => "Unknown Level"
        };
    }

    public string GetSummary()
    {
        return LevelId switch
        {
            LevelId.Flower => "Collect pollen from the yellow flower, then carry it to one of the bright flowers to complete each pollination cycle.",
            LevelId.Grass => "Guide the ground beetle through the grass habitat and eat all three green lacewings.",
            LevelId.Night => "Sweep the flashlight across the night habitat and record beetles and fireflies.",
            _ => Summary
        };
    }

    public string GetGoal()
    {
        return LevelId switch
        {
            LevelId.Flower => "Complete 2 pollination cycles by touching the yellow flower, then a bright flower.",
            LevelId.Grass => "Eat all 3 green lacewings.",
            LevelId.Night => "Photograph 2 beetles and 2 fireflies.",
            _ => Goal
        };
    }

    private void OnSelected()
    {
        owner?.SelectLevelCard(this);
    }
}
