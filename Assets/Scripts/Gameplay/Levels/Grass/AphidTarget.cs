using UnityEngine;

public class AphidTarget : MonoBehaviour
{
    public GameObject AlertIcon;
    public GameObject CapturedVisual;

    public bool IsCaptured { get; private set; }
    public bool IsAlerted { get; private set; }

    public void SetAlerted(bool isAlerted)
    {
        if (IsCaptured)
        {
            IsAlerted = false;
            if (AlertIcon != null)
            {
                AlertIcon.SetActive(false);
            }

            return;
        }

        IsAlerted = isAlerted;

        if (AlertIcon != null)
        {
            AlertIcon.SetActive(isAlerted);
        }
    }

    public void Capture()
    {
        if (IsCaptured)
        {
            return;
        }

        IsCaptured = true;
        IsAlerted = false;

        if (AlertIcon != null)
        {
            AlertIcon.SetActive(false);
        }

        if (CapturedVisual != null)
        {
            CapturedVisual.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
