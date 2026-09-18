using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DictionaryUIManager : SingletonMonoBehaviour<DictionaryUIManager>
{
    public Button PreviousButton;
    public Button NextButton;
    public Button ReviewButton;
    public List<Button> CloseButtons = new();
    public List<GameObject> DictionaryUIs = new();
    public AudioSource audioSource;

    private int currentIndex;

    private void Start()
    {
        CacheCloseButtons();

        BindButton(PreviousButton, OnPreviousClicked);
        BindButton(NextButton, OnNextClicked);
        BindButton(ReviewButton, OnReviewClicked);

        foreach (Button closeButton in CloseButtons)
        {
            BindButton(closeButton, HideDictionary);
        }

        SetButtonLabel(ReviewButton, "Continue");
        HideDictionary();
    }

    private void Update()
    {
        if (InputManager.Instance != null && InputManager.Instance.IsOpenDictionaryPressed)
        {
            ToggleDictionary();
        }
    }

    public void ToggleDictionary()
    {
        if (DictionaryUIs.Count == 0)
        {
            return;
        }

        if (!IsDictionaryOpen())
        {
            currentIndex = 0;
            UpdatePage();
            PlayPageFlipSound();
        }
        else
        {
            HideDictionary();
            PlayPageFlipSound();
        }
    }

    public void HideDictionary()
    {
        SetAllPagesActive(false);
        SetNavigationVisible(false, false, false);
    }

    private void UpdatePage()
    {
        SetAllPagesActive(false);

        if (currentIndex < 0 || currentIndex >= DictionaryUIs.Count)
        {
            currentIndex = 0;
        }

        if (DictionaryUIs[currentIndex] == null)
        {
            return;
        }

        DictionaryUIs[currentIndex].SetActive(true);

        bool isLastPage = currentIndex >= DictionaryUIs.Count - 1;
        SetNavigationVisible(currentIndex > 0, !isLastPage, isLastPage);
    }

    private void OnNextClicked()
    {
        if (currentIndex < DictionaryUIs.Count - 1)
        {
            currentIndex++;
            UpdatePage();
            PlayPageFlipSound();
        }
    }

    private void OnPreviousClicked()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdatePage();
            PlayPageFlipSound();
        }
    }

    private void OnReviewClicked()
    {
        HideDictionary();
        PlayPageFlipSound();

        if (NotebookSceneManager.Instance != null)
        {
            NotebookSceneManager.Instance.OpenEndSequence();
            return;
        }

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.RequestNotebookPage(NotebookPageId.EndSequence);
        }

        SceneManager.LoadScene(GameSceneNames.Notebook);
    }

    private void PlayPageFlipSound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    private void SetAllPagesActive(bool isActive)
    {
        foreach (GameObject ui in DictionaryUIs)
        {
            if (ui != null)
            {
                ui.SetActive(isActive);
            }
        }
    }

    private void SetNavigationVisible(bool previousVisible, bool nextVisible, bool reviewVisible)
    {
        if (PreviousButton != null)
        {
            PreviousButton.gameObject.SetActive(previousVisible);
        }

        if (NextButton != null)
        {
            NextButton.gameObject.SetActive(nextVisible);
        }

        if (ReviewButton != null)
        {
            ReviewButton.gameObject.SetActive(reviewVisible);
        }
    }

    private void CacheCloseButtons()
    {
        CloseButtons.RemoveAll(button => button == null);
        if (CloseButtons.Count > 0)
        {
            return;
        }

        foreach (GameObject page in DictionaryUIs)
        {
            if (page == null)
            {
                continue;
            }

            foreach (Button button in page.GetComponentsInChildren<Button>(true))
            {
                if (button != null && button.name == "CloseButton" && !CloseButtons.Contains(button))
                {
                    CloseButtons.Add(button);
                }
            }
        }
    }

    private bool IsDictionaryOpen()
    {
        foreach (GameObject dictionaryUi in DictionaryUIs)
        {
            if (dictionaryUi != null && dictionaryUi.activeSelf)
            {
                return true;
            }
        }

        return false;
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
