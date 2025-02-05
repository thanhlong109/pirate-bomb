using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PirateNPC : IDamagable
{
    [SerializeField] protected PirateNPCData NPCData;
    [SerializeField] private GameObject surpriseSign;
    [SerializeField] public float surpriseTime = 0.25f;
    [SerializeField] public Vector2 surpriseOffset;
    [SerializeField] private DamageDealer damageDealer;

    protected Rigidbody2D rb;
    public NPC_STATES state = NPC_STATES.IDLE;
    private int currentDirection = 1;

    private new void Awake()
    {
        base.Awake();
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
    }

    private void FixedUpdate()
    {
        if(isDead) return;
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
            state = NPC_STATES.IDLE;
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

    public bool CanAttack => NPCData.canAttack;

}

public enum NPC_STATES
{
    IDLE, BOOM_DETECTED, PLAYER_DETECTED
}
