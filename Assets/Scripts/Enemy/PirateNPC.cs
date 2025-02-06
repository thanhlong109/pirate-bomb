using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PirateNPC : MonoBehaviour, IDamagable
{
    [SerializeField] protected PirateNPCData NPCData;
    [SerializeField] private GameObject surpriseSign;
    [SerializeField] public float surpriseTime = 0.25f;
    [SerializeField] public Vector2 surpriseOffset;
    [SerializeField] private DamageDealer damageDealer;

    [Header("NPC Health Settings")]
    [SerializeField] protected int maxHealth = 20;
    [SerializeField] protected int currentHealth = 20;
    [SerializeField] protected bool isDead = false;
    [SerializeField] protected float damagePreventTime = 0.15f;
    private bool isPreventDamage = false;

    protected Animator animator;


    protected Rigidbody2D rb;
    public NPC_STATES currentState = NPC_STATES.SLEEPING;
    public NPC_ACTION currentAction = NPC_ACTION.FREE;
    private int currentDirection = 1;

    private GameObject currentTarget = null;

    private  void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        damageDealer = GetComponentInChildren<DamageDealer>();
    }

    private void Update()
    {
        if(isDead) return;
        UpdateAnimations();
        FlipCharacter();
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsAwakened", currentState != NPC_STATES.SLEEPING);
    }

    private void FixedUpdate()
    {
        if(isDead || currentState == NPC_STATES.SLEEPING) return;
        switch (currentAction)
        {
            case NPC_ACTION.FREE:
                {
                    currentTarget = null;
                    break;
                }
            case NPC_ACTION.BOOM_DETECTED:
                {
                    if(currentTarget != null)
                    {
                        MoveToBomb(currentTarget, () =>
                        {
                            HandleBomb(currentTarget);
                        });
                    }
                    break;
                }
            case NPC_ACTION.PLAYER_DETECTED:
                {
                    if(currentTarget != null)
                    {
                        ChasePlayer(() => { StartAttackPlayer(); });
                    }
                    break;
                }
        }
    }



    public int AttackDamage => NPCData.AttackDamage;
    public abstract void HandleBomb(GameObject bomb);
    public void ChasePlayer(System.Action onReachPlayer)
    {
        float distance = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distance > NPCData.AttackRange)
        {
            float direction = Mathf.Sign(PlayerController.Instance.transform.position.x - transform.position.x);
            rb.velocity = new Vector2(direction * NPCData.Speed, rb.velocity.y);
        }
        else
        {
            onReachPlayer.Invoke();
        }
    }

    private void FallSleep()
    {
        if(currentAction == NPC_ACTION.FREE)
        {
            currentState = NPC_STATES.SLEEPING;
        }
    }
    public void StartAttackPlayer()
    {
        animator.SetBool("IsAttack", true);
    }

    private void ActiveSensor()
    {
        damageDealer.SetActive(true);
    }

    private void InactiveSensor()
    {
        damageDealer.SetActive(false);
        animator.SetBool("IsAttack", false);
    }

    public void AttackPlayer()
    {
        PlayerController.Instance.TakeDamage(NPCData.AttackDamage);
    }

    public void ShowSurprise(NPC_ACTION action)
    {
        surpriseSign.transform.position = new Vector3(transform.position.x + surpriseOffset.x, transform.position.y + surpriseOffset.y);
        surpriseSign.SetActive(true);
        StartCoroutine(HideSurprise(action));
    }
    IEnumerator HideSurprise(NPC_ACTION action)
    {
        yield return new WaitForSeconds(surpriseTime);
        surpriseSign.SetActive(false);
        currentAction = action;
        currentState = NPC_STATES.AWAKENED;
    }

    public void SetAction(NPC_ACTION action, GameObject target)
    {
        SetAction(action, target, false);
    }
    public void SetAction(NPC_ACTION action,GameObject target, bool isOverride)
    {
        
        if(currentAction != action)
        {
            if (currentAction > action && !isOverride) return;
            if(currentState == NPC_STATES.SLEEPING)
            {
                ShowSurprise(action);
            }
            else
            {
                currentAction = action;
            }
            currentTarget = target;
        }
    }

    public void MoveToBomb(GameObject bomb, System.Action onReachBomb)
    {
        float distance = Vector2.Distance(transform.position, bomb.transform.position);

        if (distance > 0.1f)
        {
            float direction = Mathf.Sign(bomb.transform.position.x - transform.position.x);
            rb.velocity = new Vector2(direction * NPCData.Speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            onReachBomb.Invoke();
        }
    }

    private void FlipCharacter()
    {
        float direction = rb.velocity.x;
        if (direction * currentDirection < 0)
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
        Debug.Log("NPC dead");
        isDead = true;
    }

    public bool CanAttack => NPCData.canAttack;

    public int CurrentHealth => currentHealth;

    public bool IsDead => isDead;
}

public enum NPC_STATES
{
    SLEEPING ,AWAKENED
}

public enum NPC_ACTION
{
    FREE, PLAYER_DETECTED, BOOM_DETECTED
}
