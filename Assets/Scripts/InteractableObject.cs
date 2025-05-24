using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{
    public GameObject Prefab;
    public Button InteractButton;
    public InteractableTime ObjectInteractableTime;

    private void Start()
    {
        InteractButton.gameObject.SetActive(false);
        InteractButton.GetComponentInParent<Canvas>().worldCamera = Camera.main;
        InteractButton.onClick.AddListener(OnInteracted);
    }

    public void ShowUI()
    {
        InteractButton.gameObject.SetActive(true);
    }

    public void HideUI()
    {
        InteractButton.gameObject.SetActive(false);
    }

    public void OnInteracted()
    {
        if (ObjectInteractableTime == InteractableTime.Night && !TimeManager.Instance.IsNightTime)
        {
            Notice.Instance.ShowNightNotice();
            return;
        }
        else if (ObjectInteractableTime == InteractableTime.Day && TimeManager.Instance.IsNightTime)
        {
            Notice.Instance.ShowDayNotice();
            return;
        }

        Player.Instance.PlayerMovement.IsInteracting = true;
        GameObject go = Instantiate(Prefab);
        go.GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
