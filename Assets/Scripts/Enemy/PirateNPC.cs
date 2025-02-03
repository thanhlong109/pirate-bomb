using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PirateNPC : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 1f;
    

    private Rigidbody2D rb;
    public NPC_STATES state = NPC_STATES.IDLE;
    private GameObject bombDetected;
    private System.Action onReachBomb;
    private Animator animator;
    public abstract void HandleBomb(GameObject bomb);
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimations();
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
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            onReachBomb.Invoke();
            state = NPC_STATES.IDLE;
        }
    }

}

public enum NPC_STATES
{
    IDLE, MOVE_TO_BOMB
}
