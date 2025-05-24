using UnityEngine;

public abstract class SpecialScene : MonoBehaviour
{
    public void Exit()
    {
        Player.Instance.PlayerMovement.IsInteracting = false;
        Destroy(gameObject);
    }
}
