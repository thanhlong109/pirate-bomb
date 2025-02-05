using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerDetector : MonoBehaviour
{
    private PirateNPC pirateNPC;

    private void Start()
    {
        pirateNPC = GetComponentInParent<PirateNPC>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pirateNPC.CanAttack) return;
        if (pirateNPC.state == NPC_STATES.IDLE)
        {
            pirateNPC.ShowSurprise();
            StartCoroutine(DelayToAttackPlayer());
        }
        
    }

    private IEnumerator DelayToAttackPlayer()
    {
        yield return new WaitForSeconds(pirateNPC.surpriseTime);
        pirateNPC.state = NPC_STATES.PLAYER_DETECTED;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(PlayerController.Instance.IsDead || !pirateNPC.CanAttack) return;
        if (pirateNPC.state != NPC_STATES.PLAYER_DETECTED) return;
        pirateNPC.ChasePlayer(() => { pirateNPC.StartAttackPlayer(); });
    }

    private void OnTriggerExit2D(Collider2D collision)
    {   
        if (!pirateNPC.CanAttack) return;
        pirateNPC.state = NPC_STATES.IDLE;
    }
}
