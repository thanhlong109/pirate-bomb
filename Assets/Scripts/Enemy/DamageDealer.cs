using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class DamageDealer : MonoBehaviour
{
    private PolygonCollider2D polygonCollider2D;
    private PirateNPC pirateNPC;

    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();   
        pirateNPC = GetComponentInParent<PirateNPC>();
    }

    void Start()
    {
        polygonCollider2D.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        pirateNPC.AttackPlayer();
    }

    public void SetActive(bool active)
    {
        polygonCollider2D.enabled = active;
    }
}
