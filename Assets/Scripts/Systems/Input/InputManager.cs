using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : SingletonMonoBehaviour<InputManager>
{
    [HideInInspector] public Vector2 Movement;
    [HideInInspector] public bool IsInteractPressed;
    [HideInInspector] public bool IsPhotoPressed;
    [HideInInspector] public bool IsOpenDictionaryPressed;
    [HideInInspector] public bool IsPausePressed;
    [HideInInspector] public bool IsPrimaryClickPressed;
    [HideInInspector] public Vector2 PointerScreenPosition;

    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction photoAction;
    private InputAction openDictionaryAction;

    protected override void Awake()
    {
        base.Awake();

        playerInput = GetComponent<PlayerInput>();
        moveAction = FindAction("Move");
        interactAction = FindAction("Interact");
        photoAction = FindAction("Photo");
        openDictionaryAction = FindAction("OpenDictionary");
    }

    private void Update()
    {
        Movement = moveAction != null ? moveAction.ReadValue<Vector2>() : ReadMovementFallback();
        PointerScreenPosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        IsPrimaryClickPressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        // Keyboard fallbacks keep the project usable even before the Input Actions asset is updated in the editor.
        IsInteractPressed = WasPressed(interactAction) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F);
        IsPhotoPressed = WasPressed(photoAction) || Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0);
        IsOpenDictionaryPressed = WasPressed(openDictionaryAction) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.J);
        IsPausePressed = Input.GetKeyDown(KeyCode.Escape);
    }

    private InputAction FindAction(string actionName)
    {
        return playerInput != null ? playerInput.actions.FindAction(actionName, false) : null;
    }

    private static bool WasPressed(InputAction action)
    {
        return action != null && action.WasPressedThisFrame();
    }

    private static Vector2 ReadMovementFallback()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x -= 1f;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            movement.x += 1f;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            movement.y -= 1f;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            movement.y += 1f;
        }

        return movement.normalized;
    }
}
