using UnityEngine;

public class FlowerPlayerAgent : MonoBehaviour
{
    public FlowerPollinationController Controller;

    private void Start()
    {
        if (Controller == null)
        {
            Controller = FindAnyObjectByType<FlowerPollinationController>();
        }
    }

    public void TryVisitFlower(FlowerTarget flowerTarget)
    {
        Controller?.TryVisitFlower(flowerTarget);
    }
}
