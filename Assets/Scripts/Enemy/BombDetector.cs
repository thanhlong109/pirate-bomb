using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BombDetector : MonoBehaviour
{

    private PirateNPC NPC;
    private void Start()
    {
        NPC = GetComponentInParent<PirateNPC>();
        if (NPC == null)
        {
            Debug.LogError("BombDetector không tìm thấy PirateNPC trong cha!");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (NPC.IsDead) return;
        if(NPC.state == NPC_STATES.BOOM_DETECTED)
        {
            NPC.MoveToBomb(collision.gameObject, () =>
            {
                NPC.HandleBomb(collision.gameObject);
            });
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (NPC.state != NPC_STATES.BOOM_DETECTED)
        {
            NPC.ShowSurprise();
            StartCoroutine(DelayToHandleBomb());
        }
        else
        {
            NPC.state = NPC_STATES.BOOM_DETECTED;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        NPC.state = NPC_STATES.IDLE;   
    }

    private IEnumerator DelayToHandleBomb()
    {
        yield return new WaitForSeconds(NPC.surpriseTime);
        NPC.state = NPC_STATES.BOOM_DETECTED;
    }
}
