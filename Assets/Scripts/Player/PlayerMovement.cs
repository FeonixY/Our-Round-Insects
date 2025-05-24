using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MaxSpeed;
    [HideInInspector]
    public bool IsInteracting;
    private Rigidbody m_rigidbody;

    private Camera MainCamera => Camera.main;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (IsInteracting)
            return;
        Move();
    }

    private void Move()
    {
        if (InputManager.Instance.Movement != Vector2.zero)
        {
            Vector3 forward = MainCamera.transform.forward;
            Vector3 right = MainCamera.transform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * InputManager.Instance.Movement.y + right * InputManager.Instance.Movement.x;
            m_rigidbody.linearVelocity = MaxSpeed * desiredMoveDirection.normalized;
        }
    }
}
