using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] public PlayerActions PlayerActions;
    InputAction moveAction;
    InputAction attackAction;
    InputAction switchAction;
    InputAction debugAction;
    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        switchAction = InputSystem.actions.FindAction("Switch");
    }

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        PlayerActions.Move(moveValue);

        if (attackAction.WasPressedThisFrame())
        {
            PlayerActions.Attack();
        }
        if (switchAction.WasPressedThisFrame())
        {
            PlayerActions.modeSwitch();
        }
    }


}
