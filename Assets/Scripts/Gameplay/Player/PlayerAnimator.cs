using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (animator == null || InputManager.Instance == null)
        {
            return;
        }

        Vector2 movement = InputManager.Instance.Movement;
        animator.SetBool("IsWalkingForward", movement.y > 0);
        animator.SetBool("IsWalkingBackward&Side", movement.y < 0 || movement.x != 0);
        animator.transform.localScale = new Vector3(movement.x < 0 ? -1 : 1, 1, 1);
    }
}
