using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DictionaryUIManager : SingletonMonoBehaviour<DictionaryUIManager>
{
    public Button PreviousButton;
    public Button NextButton;
    public Button ReviewButton;

    public List<GameObject> DictionaryUIs = new();
    public List<Image> InsectImages = new();
    public List<Image> InformationImages = new();
    public List<InsectData> InsectDatas = new();

    public AudioSource audioSource;

    private int currentIndex = 0;

    private void Start()
    {
        if (PreviousButton != null)
        {
            PreviousButton.onClick.AddListener(OnPreviousClicked);
        }
        if (NextButton != null)
        {
            NextButton.onClick.AddListener(OnNextClicked);
        }
        if (ReviewButton != null)
        {
            ReviewButton.onClick.AddListener(OnReviewClicked);
        }
    }

    private void Update()
    {
        if (InputManager.Instance.IsOpenDictionaryPressed)
        {
            if (DictionaryUIs.TrueForAll((x) => !x.activeSelf))
            {
                currentIndex = 0;
                UpdatePage();
                PlayPageFlipSound();
            }
            else
            {
                foreach (var ui in DictionaryUIs)
                {
                    ui.SetActive(false);
                }
                PreviousButton.gameObject.SetActive(false);
                NextButton.gameObject.SetActive(false);
                ReviewButton.gameObject.SetActive(false);
                PlayPageFlipSound();
            }
        }
    }

    private void UpdatePage()
    {
        foreach (var ui in DictionaryUIs)
        {
            ui.SetActive(false);
        }
        DictionaryUIs[currentIndex].SetActive(true);

        if (PreviousButton != null)
        {
            PreviousButton.gameObject.SetActive(currentIndex > 0);
        }
        if (NextButton != null)
        {
            NextButton.gameObject.SetActive(currentIndex < DictionaryUIs.Count - 1);
        }

        bool allCollected = true;

        for (int i = 0; i < InsectDatas.Count; i++)
        {
            var insectData = InsectDatas[i];
            bool found = false;
            foreach (var uiData in InsectDataManager.Instance.InsectDataList)
            {
                if (uiData == insectData)
                {
                    InsectImages[i].color = Color.white;
                    InsectImages[i].sprite = insectData.Image;
                    InformationImages[i].color = Color.white;
                    InformationImages[i].sprite = insectData.InformationImage;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                InsectImages[i].color = new Color(1, 1, 1, 0);
                InformationImages[i].color = new Color(1, 1, 1, 0);
                allCollected = false;
            }
        }

        ReviewButton.gameObject.SetActive(allCollected);
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
        SceneManager.LoadScene("EndScene");
    }

    private void PlayPageFlipSound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
