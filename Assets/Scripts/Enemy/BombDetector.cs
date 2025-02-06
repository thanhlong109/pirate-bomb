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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (NPC.IsDead) return;
        NPC.SetAction(NPC_ACTION.BOOM_DETECTED, collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        NPC.SetAction(NPC_ACTION.FREE, null, true);   
    }
}
