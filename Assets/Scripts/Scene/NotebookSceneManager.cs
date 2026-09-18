using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NotebookSceneManager : MonoBehaviour
{
    public static NotebookSceneManager Instance { get; private set; }

    [Header("Shared Pages")]
    public GameObject IntroRoot;
    public GameObject LevelSelectRoot;
    public GameObject LevelResultRoot;
    public GameObject FinalReviewRoot;
    public GameObject EndRoot;

    [Header("Intro")]
    public List<GameObject> IntroPages = new();
    public Button IntroNextButton;
    public Button IntroPreviousButton;
    public Button IntroStartButton;
    public TMP_Text IntroHintText;

    [Header("Level Select")]
    public List<LevelSelectCardView> LevelCards = new();
    public TMP_Text SelectedLevelTitleText;
    public TMP_Text SelectedLevelSummaryText;
    public TMP_Text SelectedLevelGoalText;
    public TMP_Text SelectedLevelStatusText;
    public Image SelectedLevelPreviewImage;
    public Button EnterSelectedLevelButton;
    public GameObject SelectedLevelPanel;
    public Button NotebookButton;
    public Button FinalReviewButton;
    public Button BackButton;

    [Header("Level Result")]
    public TMP_Text ResultHeaderText;
    public TMP_Text ResultSummaryText;
    public TMP_Text ResultStatsText;
    public Button ResultContinueButton;
    public Button ResultBackButton;

    [Header("Final Review")]
    public TMP_Text FinalHeaderText;
    public TMP_Text FinalSummaryText;
    public Button FinalReturnToSelectButton;
    public Button FinalMainMenuButton;
    public Button FinalQuitButton;

    private int currentIntroPageIndex;
    private int currentEndPageIndex;
    private LevelSelectCardView selectedLevelCard;
    private readonly List<GameObject> endPages = new();
    private Button endNextButton;
    private Button endPreviousButton;
    private Button endFinishButton;
    private TMP_Text endTitleText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EnsureEndSequenceUi();
        BindButtons();
        SubscribeProgress();
        InitializeLevelCards();
        OpenRequestedPage();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.ProgressChanged -= RefreshNotebookData;
        }
    }

    private void Update()
    {
        if (IsPageActive(NotebookPageId.Intro))
        {
            if (ShouldAdvancePageFromPointerClick() && currentIntroPageIndex < IntroPages.Count - 1)
            {
                NextIntroPage();
            }

            if (ShouldAdvancePageFromKeyboard() && currentIntroPageIndex < IntroPages.Count - 1)
            {
                NextIntroPage();
            }

            return;
        }

        if (!IsPageActive(NotebookPageId.EndSequence))
        {
            return;
        }

        if (ShouldAdvancePageFromPointerClick() && currentEndPageIndex < endPages.Count - 1)
        {
            NextEndPage();
        }

        if (ShouldAdvancePageFromKeyboard() && currentEndPageIndex < endPages.Count - 1)
        {
            NextEndPage();
        }
    }

    public void OpenLevel(LevelId levelId)
    {
        SceneManager.LoadScene(GameSceneNames.GetLevelSceneName(levelId));
    }

    public void SelectLevelCard(LevelSelectCardView card)
    {
        if (card == null)
        {
            return;
        }

        selectedLevelCard = card;
        RefreshLevelSelectPage();
    }

    public void OpenDictionary()
    {
        if (DictionaryUIManager.Instance != null)
        {
            DictionaryUIManager.Instance.ToggleDictionary();
        }
    }

    public void OpenFinalReview()
    {
        SetActivePage(NotebookPageId.FinalReview);
    }

    public void OpenEndSequence()
    {
        currentEndPageIndex = 0;
        SetActivePage(NotebookPageId.EndSequence);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(GameSceneNames.MainMenu);
    }

    public void GoToLevelSelect()
    {
        SetActivePage(NotebookPageId.LevelSelect);
    }

    public void NextIntroPage()
    {
        if (currentIntroPageIndex >= IntroPages.Count - 1)
        {
            return;
        }

        currentIntroPageIndex++;
        RefreshIntroPage();
    }

    public void PreviousIntroPage()
    {
        if (currentIntroPageIndex <= 0)
        {
            return;
        }

        currentIntroPageIndex--;
        RefreshIntroPage();
    }

    public void FinishIntro()
    {
        SetActivePage(NotebookPageId.LevelSelect);
    }

    public void NextEndPage()
    {
        if (currentEndPageIndex >= endPages.Count - 1)
        {
            return;
        }

        currentEndPageIndex++;
        RefreshEndPage();
    }

    public void PreviousEndPage()
    {
        if (currentEndPageIndex <= 0)
        {
            return;
        }

        currentEndPageIndex--;
        RefreshEndPage();
    }

    public void FinishEndSequence()
    {
        QuitGame();
    }

    private void BindButtons()
    {
        BindButton(IntroNextButton, NextIntroPage);
        BindButton(IntroPreviousButton, PreviousIntroPage);
        BindButton(IntroStartButton, FinishIntro);

        BindButton(NotebookButton, OpenDictionary);
        BindButton(FinalReviewButton, OpenFinalReview);
        BindButton(BackButton, BackToMainMenu);
        BindButton(EnterSelectedLevelButton, OpenSelectedLevel);

        BindButton(ResultContinueButton, HandleResultContinue);
        BindButton(ResultBackButton, BackToMainMenu);

        BindButton(FinalReturnToSelectButton, GoToLevelSelect);
        BindButton(FinalMainMenuButton, BackToMainMenu);
        BindButton(FinalQuitButton, QuitGame);

        BindButton(endNextButton, NextEndPage);
        BindButton(endPreviousButton, PreviousEndPage);
        BindButton(endFinishButton, FinishEndSequence);
    }

    private void SubscribeProgress()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.ProgressChanged += RefreshNotebookData;
        }
    }

    private void InitializeLevelCards()
    {
        foreach (LevelSelectCardView card in LevelCards)
        {
            if (card != null)
            {
                card.Initialize(this);
            }
        }

        if (selectedLevelCard == null)
        {
            selectedLevelCard = GetDefaultLevelCard();
        }
    }

    private void OpenRequestedPage()
    {
        NotebookPageId requestedPage = GameProgressManager.Instance != null
            ? GameProgressManager.Instance.RequestedNotebookPage
            : NotebookPageId.Intro;

        SetActivePage(requestedPage);

        if (GameProgressManager.Instance != null && GameProgressManager.Instance.OpenDictionaryOnNotebookOpen)
        {
            StartCoroutine(OpenDictionaryNextFrame());
        }
    }

    private void SetActivePage(NotebookPageId pageId)
    {
        SetRootActive(IntroRoot, pageId == NotebookPageId.Intro);
        SetRootActive(LevelSelectRoot, pageId == NotebookPageId.LevelSelect);
        SetRootActive(LevelResultRoot, pageId == NotebookPageId.LevelResult);
        SetRootActive(FinalReviewRoot, pageId == NotebookPageId.FinalReview);
        SetRootActive(EndRoot, pageId == NotebookPageId.EndSequence);

        if (pageId == NotebookPageId.Intro)
        {
            currentIntroPageIndex = Mathf.Clamp(currentIntroPageIndex, 0, Mathf.Max(0, IntroPages.Count - 1));
            RefreshIntroPage();
        }
        else if (pageId == NotebookPageId.EndSequence)
        {
            currentEndPageIndex = Mathf.Clamp(currentEndPageIndex, 0, Mathf.Max(0, endPages.Count - 1));
            RefreshEndPage();
        }

        RefreshNotebookData();

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.RequestNotebookPage(pageId);
            GameProgressManager.Instance.ClearNotebookOverlayRequest();
        }
    }

    private void RefreshIntroPage()
    {
        for (int i = 0; i < IntroPages.Count; i++)
        {
            if (IntroPages[i] != null)
            {
                IntroPages[i].SetActive(i == currentIntroPageIndex);
            }
        }

        if (IntroPreviousButton != null)
        {
            IntroPreviousButton.gameObject.SetActive(currentIntroPageIndex > 0);
        }

        if (IntroNextButton != null)
        {
            IntroNextButton.gameObject.SetActive(currentIntroPageIndex < IntroPages.Count - 1);
        }

        if (IntroStartButton != null)
        {
            IntroStartButton.gameObject.SetActive(IntroPages.Count == 0 || currentIntroPageIndex == IntroPages.Count - 1);
        }

        if (IntroHintText != null)
        {
            IntroHintText.SetText(IntroPages.Count == 0 || currentIntroPageIndex == IntroPages.Count - 1
                ? "Start Observing"
                : "Click or press Space to turn the page");
        }
    }

    private void RefreshEndPage()
    {
        for (int i = 0; i < endPages.Count; i++)
        {
            if (endPages[i] != null)
            {
                endPages[i].SetActive(i == currentEndPageIndex);
            }
        }

        if (endTitleText != null)
        {
            endTitleText.SetText("Closing Notes");
        }

        if (endPreviousButton != null)
        {
            endPreviousButton.gameObject.SetActive(currentEndPageIndex > 0);
        }

        if (endNextButton != null)
        {
            endNextButton.gameObject.SetActive(currentEndPageIndex < endPages.Count - 1);
        }

        if (endFinishButton != null)
        {
            endFinishButton.gameObject.SetActive(endPages.Count == 0 || currentEndPageIndex == endPages.Count - 1);
        }
    }

    private static bool ShouldAdvancePageFromPointerClick()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return false;
        }

        return !IsPointerOverInteractiveUi();
    }

    private static bool ShouldAdvancePageFromKeyboard()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return false;
        }

        if (EventSystem.current == null)
        {
            return true;
        }

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        return selectedObject == null || selectedObject.GetComponent<Button>() == null;
    }

    private static bool IsPointerOverInteractiveUi()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        PointerEventData pointerEventData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (RaycastResult raycastResult in raycastResults)
        {
            if (raycastResult.gameObject != null && raycastResult.gameObject.GetComponentInParent<Button>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private void RefreshNotebookData()
    {
        RefreshLevelSelectPage();
        RefreshResultPage();
        RefreshFinalReviewPage();
    }

    private void RefreshLevelSelectPage()
    {
        if (selectedLevelCard == null)
        {
            selectedLevelCard = GetDefaultLevelCard();
        }

        foreach (LevelSelectCardView card in LevelCards)
        {
            if (card != null)
            {
                card.Refresh();
                card.SetSelected(card == selectedLevelCard);
            }
        }

        RefreshSelectedLevelPanel();

        if (FinalReviewButton != null)
        {
            FinalReviewButton.gameObject.SetActive(GameProgressManager.Instance != null && GameProgressManager.Instance.AreAllLevelsCompleted());
        }
    }

    private void RefreshResultPage()
    {
        LevelSummary summary = GameProgressManager.Instance != null ? GameProgressManager.Instance.LastCompletedSummary : null;

        if (summary == null)
        {
            if (ResultHeaderText != null)
            {
                ResultHeaderText.SetText("Field Notes");
            }

            if (ResultSummaryText != null)
            {
                ResultSummaryText.SetText("Finish a level and this page will record the ecological help you just provided.");
            }

            if (ResultStatsText != null)
            {
                ResultStatsText.SetText(string.Empty);
            }

            return;
        }

        if (ResultHeaderText != null)
        {
            ResultHeaderText.SetText(summary.Title);
        }

        if (ResultSummaryText != null)
        {
            ResultSummaryText.SetText(summary.SummaryText);
        }

        if (ResultStatsText != null)
        {
            ResultStatsText.SetText($"Primary Score: {summary.PrimaryScore}\nSecondary Score: {summary.SecondaryScore}");
        }
    }

    private void RefreshFinalReviewPage()
    {
        if (FinalHeaderText != null)
        {
            FinalHeaderText.SetText("Ecology Review");
        }

        if (FinalSummaryText == null)
        {
            return;
        }

        string summary = "Pollination, predation, decomposition, and indicator species all help keep the ecosystem in balance.";
        if (GameProgressManager.Instance != null && GameProgressManager.Instance.LastCompletedSummary != null)
        {
            summary += "\n\n" + GameProgressManager.Instance.LastCompletedSummary.SummaryText;
        }

        FinalSummaryText.SetText(summary);
    }

    private void HandleResultContinue()
    {
        if (GameProgressManager.Instance != null && GameProgressManager.Instance.AreAllLevelsCompleted())
        {
            SetActivePage(NotebookPageId.FinalReview);
            return;
        }

        SetActivePage(NotebookPageId.LevelSelect);
    }

    private bool IsPageActive(NotebookPageId pageId)
    {
        return pageId switch
        {
            NotebookPageId.Intro => IntroRoot != null && IntroRoot.activeSelf,
            NotebookPageId.LevelSelect => LevelSelectRoot != null && LevelSelectRoot.activeSelf,
            NotebookPageId.LevelResult => LevelResultRoot != null && LevelResultRoot.activeSelf,
            NotebookPageId.FinalReview => FinalReviewRoot != null && FinalReviewRoot.activeSelf,
            NotebookPageId.EndSequence => EndRoot != null && EndRoot.activeSelf,
            _ => false
        };
    }

    private static void SetRootActive(GameObject root, bool isActive)
    {
        if (root != null)
        {
            root.SetActive(isActive);
        }
    }

    private static void BindButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    private void OpenSelectedLevel()
    {
        if (selectedLevelCard != null)
        {
            OpenLevel(selectedLevelCard.LevelId);
        }
    }

    private void RefreshSelectedLevelPanel()
    {
        if (SelectedLevelPanel != null)
        {
            SelectedLevelPanel.SetActive(selectedLevelCard != null);
        }

        if (selectedLevelCard == null)
        {
            return;
        }

        if (SelectedLevelTitleText != null)
        {
            SelectedLevelTitleText.SetText(selectedLevelCard.GetTitle());
        }

        if (SelectedLevelSummaryText != null)
        {
            SelectedLevelSummaryText.SetText(selectedLevelCard.GetSummary());
        }

        if (SelectedLevelGoalText != null)
        {
            SelectedLevelGoalText.SetText(selectedLevelCard.GetGoal());
        }

        if (SelectedLevelStatusText != null)
        {
            SelectedLevelStatusText.SetText(selectedLevelCard.IsCompleted ? "Completed" : "Ready to explore");
        }

        if (SelectedLevelPreviewImage != null)
        {
            SelectedLevelPreviewImage.sprite = selectedLevelCard.PreviewSprite;
            SelectedLevelPreviewImage.preserveAspect = true;
        }
    }

    private LevelSelectCardView GetDefaultLevelCard()
    {
        foreach (LevelSelectCardView card in LevelCards)
        {
            if (card != null)
            {
                return card;
            }
        }

        return null;
    }

    private IEnumerator OpenDictionaryNextFrame()
    {
        yield return null;
        OpenDictionary();
    }

    private void EnsureEndSequenceUi()
    {
        if (EndRoot != null || IntroRoot == null)
        {
            return;
        }

        EndRoot = Instantiate(IntroRoot, IntroRoot.transform.parent);
        EndRoot.name = "EndRoot";
        EndRoot.SetActive(false);

        endTitleText = EndRoot.transform.Find("IntroTitle")?.GetComponent<TMP_Text>();
        endPreviousButton = EndRoot.transform.Find("IntroPreviousButton")?.GetComponent<Button>();
        endNextButton = EndRoot.transform.Find("IntroNextButton")?.GetComponent<Button>();
        endFinishButton = EndRoot.transform.Find("IntroStartButton")?.GetComponent<Button>();

        SetButtonLabel(endPreviousButton, "Back");
        SetButtonLabel(endNextButton, "Next");
        SetButtonLabel(endFinishButton, "Finish");

        endPages.Clear();

        List<Sprite> endSprites = new();
        for (int index = 1; index <= 8; index++)
        {
            Sprite sprite = Resources.Load<Sprite>($"Dialogues/EndSceneDialogue_{index}");
            if (sprite == null)
            {
                break;
            }

            endSprites.Add(sprite);
        }

        GameObject templatePage = EndRoot.transform.Find("IntroPage_1")?.gameObject;
        if (templatePage == null)
        {
            return;
        }

        List<GameObject> discoveredPages = new();
        for (int index = 1; index <= 8; index++)
        {
            Transform pageTransform = EndRoot.transform.Find($"IntroPage_{index}");
            if (pageTransform != null)
            {
                discoveredPages.Add(pageTransform.gameObject);
            }
        }

        while (discoveredPages.Count < endSprites.Count)
        {
            GameObject extraPage = Instantiate(templatePage, EndRoot.transform);
            extraPage.name = $"IntroPage_{discoveredPages.Count + 1}";
            discoveredPages.Add(extraPage);
        }

        for (int index = 0; index < discoveredPages.Count; index++)
        {
            GameObject page = discoveredPages[index];
            bool hasSprite = index < endSprites.Count;
            page.name = $"EndPage_{index + 1}";
            page.SetActive(false);

            if (!hasSprite)
            {
                continue;
            }

            Image artworkImage = page.transform.Find("Artwork")?.GetComponent<Image>();
            if (artworkImage != null)
            {
                artworkImage.sprite = endSprites[index];
                artworkImage.preserveAspect = true;
            }

            endPages.Add(page);
        }
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private static void SetButtonLabel(Button button, string label)
    {
        if (button == null)
        {
            return;
        }

        TMP_Text labelText = button.GetComponentInChildren<TMP_Text>(true);
        if (labelText != null)
        {
            labelText.SetText(label);
        }
    }
}
