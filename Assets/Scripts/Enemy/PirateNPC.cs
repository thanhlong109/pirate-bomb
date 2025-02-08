using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PirateNPC : MonoBehaviour, IDamagable
{
    [SerializeField] protected PirateNPCData NPCData;
    [SerializeField] private GameObject surpriseSign;
    [SerializeField] public float surpriseTime = 0.25f;
    [SerializeField] public Vector2 surpriseOffset;
    [SerializeField] private DamageDealer damageDealer;

    [Header("NPC Health Info")]
    [SerializeField] private int currentHealth = 20;
    [SerializeField] protected bool isDead = false;
    [SerializeField] protected float damagePreventTime = 0.15f;
    private bool isPreventDamage = false;

    protected Animator animator;

    protected Rigidbody2D rb;
    public NPC_STATES currentState = NPC_STATES.SLEEPING;
    protected int currentDirection = 1;

    private ITargetable currentTarget = null;
    private List<ITargetable> targets = new List<ITargetable>();

    protected  void Awake()
    {
        currentHealth = NPCData.Health;
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

        // take next target
        if(currentTarget == null && targets.Count > 0 )
        {
            currentTarget = targets.OrderByDescending(t => t.Priority).FirstOrDefault();
            if(currentTarget != null && currentTarget.Action != NPC_ACTION.PLAYER_DETECTED)
            {
                targets.Remove(currentTarget);
            }
        }
        if (currentTarget == null) return;
        if (!currentTarget.GameObject.activeSelf)
        {
            currentTarget = null;
            return;
        }
        switch (currentTarget.Action)
        {
            case NPC_ACTION.FREE:
                {
                    break;
                }
            case NPC_ACTION.BOOM_DETECTED:
                {
                    if(currentTarget != null)
                    {
                        MoveToBomb(currentTarget.GameObject, () =>
                        {
                            HandleBomb(currentTarget.GameObject);
                            currentTarget = null;
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

    public void AddTarget(ITargetable target)
    {
        if(currentState == NPC_STATES.SLEEPING)
        {
            ShowSurprise();
        }
        
        if(currentTarget != null && currentTarget.Priority < target.Priority)
        {
            currentTarget = target;
        }
        else
        {
            targets.Add(target);
        }
    }

    public void RemoveTarget(ITargetable target)
    {
        if (targets.Contains(target))
        {
            targets.Remove(target);
        }
        if(currentTarget == target) currentTarget = null;
    }

    public int AttackDamage => NPCData.AttackDamage;
    public abstract void HandleBomb(GameObject bomb);
    public void ChasePlayer(System.Action onReachPlayer)
    {
        if (PlayerController.Instance.IsDead)
        {
            RemoveTarget(currentTarget); return;
        }
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
        if(currentTarget == null)
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

    public void ShowSurprise()
    {
        surpriseSign.transform.position = new Vector3(transform.position.x + surpriseOffset.x, transform.position.y + surpriseOffset.y);
        surpriseSign.SetActive(true);
        StartCoroutine(HideSurprise());
    }
    IEnumerator HideSurprise()
    {
        yield return new WaitForSeconds(surpriseTime);
        surpriseSign.SetActive(false);
        currentState = NPC_STATES.AWAKENED;
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
