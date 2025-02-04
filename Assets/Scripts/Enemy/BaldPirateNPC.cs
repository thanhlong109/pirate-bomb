using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaldPirateNPC : PirateNPC
{
    [SerializeField] private float bombKickForce = 300f;
    [SerializeField] private float bombKickYAddForce = 0.1f;
    public override void HandleBomb(GameObject bomb)
    {
        MoveToBomb(bomb, () => {
            KickTheBomb(bomb);
        });
    }

    public override void OnDead()
    {
        Debug.Log("NPC dead");
    }

    private void KickTheBomb(GameObject bomb)
    {
        Vector2 direction = bomb.transform.position - transform.position;
        direction = new Vector2 (direction.x, bombKickYAddForce);
        bomb.GetComponent<Bomb>().AddForce(direction.normalized * bombKickForce);
    }
    
}
