using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IDamagable, ITargetable
{
    public static PlayerController Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float bombCooldown = 1f;
    [SerializeField] private float minThrowForce = 0f;
    [SerializeField] private float maxThrowForce = 10f;
    [SerializeField] private float chargeTime = 2f;
    [SerializeField] private int priority = 1;

    [Header("References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private GameObject chargeBar;

    [Header("Health Settings")]
    [SerializeField] protected int maxHealth = 20;
    [SerializeField] protected int currentHealth = 20;
    [SerializeField] protected bool isDead = false;
    [SerializeField] protected float damagePreventTime = 0.15f;
    private bool isPreventDamage = false;

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

    private  void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(isDead) return;
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
        if (isDead) return;
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

    public int CurrentHealth => currentHealth;

    public bool IsDead => isDead;

    public int Priority => priority;

    public GameObject GameObject => gameObject;

    public NPC_ACTION Action => NPC_ACTION.PLAYER_DETECTED;

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

    public void TakeDamage(int damage)
    {
        if (!isDead && !isPreventDamage)
        {
            animator.SetBool("TakeDamage", true);
            currentHealth -= damage;
            isPreventDamage = true;
            StartCoroutine(WaitToDamagable());
            if (currentHealth <= 0)
            {
                OnDead();
                isDead = true;
                animator.SetBool("IsDead", true);
            }
        }
    }

    IEnumerator WaitToDamagable()
    {
        yield return new WaitForSeconds(damagePreventTime);
        animator.SetBool("TakeDamage", false);
        isPreventDamage = false;
    }

    public void OnDead()
    {
        Debug.Log("Player dead");
        isDead = true;
    }
}
