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

    protected Rigidbody2D rb;
    public NPC_STATES state = NPC_STATES.IDLE;
    private GameObject bombDetected;
    private System.Action onReachBomb;
    private int currentDirection = 1;
    
    public abstract void HandleBomb(GameObject bomb);
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

    private new void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateAnimations();
        FlipCharacter();
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case NPC_STATES.MOVE_TO_BOMB:
                {
                    MoveToBomb();
                    break;
                }
            default: break;
        }
    }

    protected void MoveToBomb(GameObject bomb, System.Action onReachBomb)
    {
        bombDetected = bomb;
        this.onReachBomb = onReachBomb;
        state = NPC_STATES.MOVE_TO_BOMB;
    }

    private void MoveToBomb()
    {   
 
        float distance = Vector2.Distance(transform.position, bombDetected.transform.position);

        if (distance > 0.1f)
        {
            float direction = Mathf.Sign(bombDetected.transform.position.x - transform.position.x);
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

}

public enum NPC_STATES
{
    IDLE, MOVE_TO_BOMB
}
