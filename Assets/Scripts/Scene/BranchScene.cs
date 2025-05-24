using UnityEngine;

public class BranchScene : SpecialScene
{
    public GameObject NextScene;

    public void Interact()
    {
        Destroy(gameObject);
        GameObject go = Instantiate(NextScene);
        go.GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
