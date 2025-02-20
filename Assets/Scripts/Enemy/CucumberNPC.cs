using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CucumberNPC : PirateNPC
{
    private Bomb currentBomb = null;

    private new void Awake()
    {
        base.Awake();
        currentDirection = -1;
    }
    public override void HandleBomb(GameObject bomb)
    {
        MoveToBomb(currentTarget.GameObject, () =>
        {
            currentBomb = bomb.GetComponent<Bomb>();
            StartBlowOutFuse(bomb);
            currentTarget = null;
        });
    }

    public override void OnDead()
    {
        base.OnDead();
        ((CapsuleCollider2D)collider2D).direction = CapsuleDirection2D.Horizontal;
    }

    private void StartBlowOutFuse(GameObject bomb)
    {
        if (currentBomb == null || currentBomb.IsExtinguished) return;
        animator.SetBool("BlowOutFuse", true);
    }

    private void BlowOutFuse()
    {
        animator.SetBool("BlowOutFuse", false);
        if (currentBomb == null) return;
        currentBomb.GetComponent<Bomb>().SetExtinguished(true);
    }
}
