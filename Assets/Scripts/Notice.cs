using System.Collections;
using UnityEngine;

public class Notice : SingletonMonoBehaviour<Notice>
{
    public GameObject DayNotice;
    public GameObject NightNotice;

    public void ShowDayNotice()
    {
        StartCoroutine(nameof(ShowNotice), DayNotice);
    }

    public void ShowNightNotice()
    {
        StartCoroutine(nameof(ShowNotice), NightNotice);
    }

    private IEnumerator ShowNotice(GameObject notice)
    {
        notice.SetActive(true);
        yield return new WaitForSeconds(3f);
        notice.SetActive(false);
    }
}
