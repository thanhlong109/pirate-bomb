using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float bombCooldown = 1f;
    [SerializeField] private float minThrowForce = 0f;
    [SerializeField] private float maxThrowForce = 10f;
    [SerializeField] private float chargeTime = 2f;

    [Header("References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private GameObject chargeBar;

    private Rigidbody2D rb;
    private bool isGrounded;
    private int currentDirection = 1;
    private float moveInput;
    private bool jumpPressed;
    private bool bombPressed;
    private bool canPlaceBomb = true;
    private Animator animator;

    private bool isCharging = false;
    private float chargeTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GetInput();
        FlipCharacter();
        UpdateAnimations();
        
        // for count time bomb pressed
        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Clamp(chargeTimer, 0f, chargeTime); 
        }
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();
        HandleBomb();
    }

    private void GetInput()
    {
        // save input to temp
        moveInput = GameInputManager.Instance.MoveDirection;
        jumpPressed = GameInputManager.Instance.IsJumpPressed;
        bombPressed = GameInputManager.Instance.IsBombPressed;
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void HandleMovement()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false; 
        }
    }

    private void HandleBomb()
    {
        if (bombPressed && canPlaceBomb)
        {
            if (!isCharging)
            {
                StartCharging();
            }
        }
        else if (!bombPressed && isCharging) 
        {
            ThrowBomb();
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeTimer = 0f;

        if (chargeBar != null)
            chargeBar.SetActive(true);
    }

    private void ThrowBomb()
    {
        if (!isCharging) return;

        isCharging = false;
        canPlaceBomb = false;

        if (chargeBar != null)
            chargeBar.SetActive(false);

        float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeTimer / chargeTime);

        Debug.Log(throwForce);

        Vector2 throwDirection = new Vector2(currentDirection, 1).normalized;

        var bomb = BombPool.Instance.GetBomb(transform.position);
        Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();

        bombRb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);

        StartCoroutine(ResetBombCooldown());
    }

    private IEnumerator ResetBombCooldown()
    {
        yield return new WaitForSeconds(bombCooldown);
        canPlaceBomb = true;
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
    }

    private void FlipCharacter()
    {
        float direction = GameInputManager.Instance.MoveDirection;
        if(direction * currentDirection < 0)
        {
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        currentDirection *= -1;
        transform.localScale = scale;
    }
}
