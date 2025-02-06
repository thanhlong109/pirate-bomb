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
        if (PlayerController.Instance.IsDead || !pirateNPC.CanAttack) return;
        pirateNPC.SetAction(NPC_ACTION.PLAYER_DETECTED,collision.gameObject);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {   
        if (!pirateNPC.CanAttack) return;
        pirateNPC.SetAction(NPC_ACTION.FREE,null,true);
    }
}
