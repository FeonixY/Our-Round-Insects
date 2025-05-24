using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : SingletonMonoBehaviour<InputManager>
{
    [HideInInspector] public Vector2 Movement;
    [HideInInspector] public bool IsInteractPressed;
    [HideInInspector] public bool IsPhotoPressed;
    [HideInInspector] public bool IsOpenDictionaryPressed;

    private PlayerInput PlayerInput;

    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction photoAction;
    private InputAction openDictionaryAction;

    protected override void Awake()
    {
        base.Awake();

        PlayerInput = GetComponent<PlayerInput>();

        moveAction = PlayerInput.actions["Move"];
        interactAction = PlayerInput.actions["Interact"];
        photoAction = PlayerInput.actions["Photo"];
        openDictionaryAction = PlayerInput.actions["OpenDictionary"];
    }

    private void Update()
    {
        Movement = moveAction.ReadValue<Vector2>();
        IsInteractPressed = interactAction.WasPressedThisFrame();
        IsPhotoPressed = photoAction.WasPressedThisFrame();
        IsOpenDictionaryPressed = openDictionaryAction.WasPressedThisFrame();
    }
}
