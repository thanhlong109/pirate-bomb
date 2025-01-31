using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    private GameInput gameInput;

    // Input Values
    private float moveDirection;
    private bool isJumpPressed;
    private bool isBombPressed;

    public float MoveDirection => moveDirection;
    public bool IsJumpPressed => isJumpPressed;
    public bool IsBombPressed => isBombPressed;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gameInput = new GameInput();
        // Setup callbacks
        gameInput.Player.Move.performed += OnMove;
        gameInput.Player.Move.canceled += OnMove;
        gameInput.Player.Jump.performed += OnJump;
        gameInput.Player.Jump.canceled += OnJump;
        gameInput.Player.PlaceBomb.performed += OnBomb;
        gameInput.Player.PlaceBomb.canceled += OnBomb;

        gameInput.Player.Enable();
    }

    private void OnDisable()
    {
        gameInput.Player.Move.performed -= OnMove;
        gameInput.Player.Move.canceled -= OnMove;
        gameInput.Player.Jump.performed -= OnJump;
        gameInput.Player.Jump.canceled -= OnJump;
        gameInput.Player.PlaceBomb.performed -= OnBomb;
        gameInput.Player.PlaceBomb.canceled -= OnBomb;
        gameInput.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<float>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    private void OnBomb(InputAction.CallbackContext context)
    {
        isBombPressed = context.ReadValueAsButton();
    }

    private void OnDestroy()
    {
        gameInput.Player.Disable();
    }
}
