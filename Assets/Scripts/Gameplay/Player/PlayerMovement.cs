using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementPlane
    {
        XZ,
        XY
    }

    public float MaxSpeed;
    public MovementPlane Plane = MovementPlane.XZ;
    [HideInInspector]
    public bool IsInteracting;
    private Rigidbody rigidbodyComponent3D;
    private Rigidbody2D rigidbodyComponent2D;

    private Camera MainCamera => Camera.main;

    private void Start()
    {
        rigidbodyComponent3D = GetComponent<Rigidbody>();
        rigidbodyComponent2D = GetComponent<Rigidbody2D>();
        ConfigureMovementPlane();
    }

    private void FixedUpdate()
    {
        if (IsInteracting)
        {
            StopMovement();

            return;
        }

        Move();
    }

    private void Move()
    {
        bool uses2DPhysics = rigidbodyComponent2D != null;
        bool missingPhysics = !uses2DPhysics && rigidbodyComponent3D == null;
        bool requiresCamera = !uses2DPhysics && Plane == MovementPlane.XZ;

        if (missingPhysics || InputManager.Instance == null || (requiresCamera && MainCamera == null))
        {
            StopMovement();
            return;
        }

        if (InputManager.Instance.Movement != Vector2.zero)
        {
            if (Plane == MovementPlane.XY)
            {
                Vector3 xyMoveDirection = new(InputManager.Instance.Movement.x, InputManager.Instance.Movement.y, 0f);
                if (uses2DPhysics)
                {
                    rigidbodyComponent2D.linearVelocity = MaxSpeed * new Vector2(xyMoveDirection.x, xyMoveDirection.y).normalized;
                }
                else if (rigidbodyComponent3D != null)
                {
                    rigidbodyComponent3D.linearVelocity = MaxSpeed * xyMoveDirection.normalized;
                }
                return;
            }

            Vector3 forward = MainCamera.transform.forward;
            Vector3 right = MainCamera.transform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 worldMoveDirection = forward * InputManager.Instance.Movement.y + right * InputManager.Instance.Movement.x;
            if (rigidbodyComponent3D != null)
            {
                rigidbodyComponent3D.linearVelocity = MaxSpeed * worldMoveDirection.normalized;
            }
            return;
        }

        StopMovement();
    }

    private void ConfigureMovementPlane()
    {
        if (rigidbodyComponent2D != null)
        {
            rigidbodyComponent2D.gravityScale = 0f;
            rigidbodyComponent2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (rigidbodyComponent3D == null)
        {
            return;
        }

        rigidbodyComponent3D.useGravity = false;
        rigidbodyComponent3D.constraints = Plane == MovementPlane.XY
            ? RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ
            : RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    private void StopMovement()
    {
        if (rigidbodyComponent2D != null)
        {
            rigidbodyComponent2D.linearVelocity = Vector2.zero;
        }

        if (rigidbodyComponent3D != null)
        {
            rigidbodyComponent3D.linearVelocity = Vector3.zero;
        }
    }
}
