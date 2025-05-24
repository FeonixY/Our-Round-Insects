using System.Collections;
using UnityEngine;

public class Insect : MonoBehaviour
{
    public InsectData InsectData;

    public virtual void OnPhotographTaken()
    {
        StartCoroutine(HandlePhotograph());
    }

    private IEnumerator HandlePhotograph()
    {
        if (!InsectDataManager.Instance.InsectDataList.Contains(InsectData))
            InsectDataManager.Instance.InsectDataList.Add(InsectData);

        yield return new WaitForSeconds(2);

        Player.Instance.PlayerMovement.IsInteracting = false;
        
        FindAnyObjectByType<LeafScene>().Exit();
    }
}
