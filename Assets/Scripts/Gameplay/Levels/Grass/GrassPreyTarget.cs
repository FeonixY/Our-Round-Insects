using UnityEngine;

public class GrassPreyTarget : MonoBehaviour
{
    public GameObject QuestionMarkRoot;
    public float QuestionMarkInterval = 2.4f;
    public float QuestionMarkDuration = 0.9f;
    public float StartOffset;

    private float timer;

    public bool IsCaptured { get; private set; }

    private void Start()
    {
        timer = -StartOffset;
        SetQuestionVisible(false);
    }

    private void Update()
    {
        if (IsCaptured)
        {
            return;
        }

        timer += Time.deltaTime;

        float cycleDuration = Mathf.Max(QuestionMarkInterval, 0.1f) + Mathf.Max(QuestionMarkDuration, 0.1f);
        if (timer >= cycleDuration)
        {
            timer -= cycleDuration;
        }

        bool showQuestion = timer >= QuestionMarkInterval;
        SetQuestionVisible(showQuestion);
    }

    public void Capture()
    {
        if (IsCaptured)
        {
            return;
        }

        IsCaptured = true;
        SetQuestionVisible(false);
        gameObject.SetActive(false);
    }

    private void SetQuestionVisible(bool isVisible)
    {
        if (QuestionMarkRoot != null)
        {
            QuestionMarkRoot.SetActive(isVisible);
        }
    }
}
