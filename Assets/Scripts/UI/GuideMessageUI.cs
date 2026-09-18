using System.Collections;
using TMPro;
using UnityEngine;

public class GuideMessageUI : SingletonMonoBehaviour<GuideMessageUI>
{
    public CanvasGroup CanvasGroup;
    public TMP_Text MessageText;
    public float DefaultDuration = 3f;

    private Coroutine activeRoutine;

    protected override void Awake()
    {
        base.Awake();
        if (CanvasGroup != null)
        {
            CanvasGroup.alpha = 0f;
        }
    }

    public void ShowMessage(string message, float duration = -1f)
    {
        if (MessageText == null || CanvasGroup == null)
        {
            return;
        }

        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }

        activeRoutine = StartCoroutine(ShowMessageRoutine(message, duration < 0f ? DefaultDuration : duration));
    }

    private IEnumerator ShowMessageRoutine(string message, float duration)
    {
        MessageText.SetText(message);
        CanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(duration);
        CanvasGroup.alpha = 0f;
        activeRoutine = null;
    }
}
