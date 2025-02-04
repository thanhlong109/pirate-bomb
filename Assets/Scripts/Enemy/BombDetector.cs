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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(NPC.state == NPC_STATES.IDLE)
        {
            NPC.ShowSurprise();
            StartCoroutine(HandleBombWithGap(other));
        }
        else
        {
            NPC.HandleBomb(other.gameObject);
        }
        
    }

    private IEnumerator HandleBombWithGap(Collider2D other)
    {
        yield return new WaitForSeconds(NPC.surpriseTime);
        NPC.HandleBomb(other.gameObject);
    }
}
