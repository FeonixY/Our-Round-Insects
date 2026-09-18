using TMPro;
using UnityEngine;

public class ObjectiveHUD : MonoBehaviour
{
    public TMP_Text TitleText;
    public TMP_Text PrimaryObjectiveText;
    public TMP_Text SecondaryObjectiveText;
    public TMP_Text StateText;
    public TMP_Text HintText;

    public void SetTitle(string title)
    {
        if (TitleText != null)
        {
            TitleText.SetText(title);
        }
    }

    public void SetObjectives(string primaryText, string secondaryText)
    {
        if (PrimaryObjectiveText != null)
        {
            PrimaryObjectiveText.SetText(primaryText);
        }

        if (SecondaryObjectiveText != null)
        {
            SecondaryObjectiveText.SetText(secondaryText);
        }
    }

    public void SetState(string stateText)
    {
        if (StateText != null)
        {
            StateText.SetText(stateText);
        }
    }

    public void SetHint(string hintText)
    {
        if (HintText != null)
        {
            HintText.SetText(hintText);
        }
    }
}
