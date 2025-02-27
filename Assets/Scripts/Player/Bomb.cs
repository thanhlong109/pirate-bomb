﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Bomb : MonoBehaviour, ITargetable
{

    [SerializeField] private float delay = 3f;
    [SerializeField] private float explosionRadius = 5f;    
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private int explosionDamage = 10;
    [SerializeField] private LayerMask affectedLayers;
    [SerializeField] private Vector2 explosionOffset = Vector2.zero;
    [SerializeField] private float addBombForce = 700f;
    [SerializeField] private int priority = 5;
    [SerializeField] private NPC_ACTION action = NPC_ACTION.BOOM_DETECTED;
    [SerializeField] private bool isExtinguished = false;

    [SerializeField] private bool hasExploded = false;  
    private Animator animator;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    private CinemachineImpulseSource impulseSource;

    public int Priority => priority;
    public bool IsExtinguished => isExtinguished;
    public GameObject GameObject => gameObject;

    public NPC_ACTION Action => action;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ExplodeAfterDelay());
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void OnEnable()
    {
        StartCoroutine(ExplodeAfterDelay());
            capsuleCollider.enabled = true;
    }

    private void OnDisable()
    {
        capsuleCollider.enabled = false;
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delay); 
        if(!isExtinguished)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;
        impulseSource.GenerateImpulse();
        animator.SetBool("Explored", true);

        Vector2 explosionCenter = (Vector2)transform.position + explosionOffset;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionCenter, explosionRadius, affectedLayers);

        foreach (Collider2D hit in colliders)
        {
            // Skip the bomb itself
            if (hit.gameObject == gameObject) continue;

            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            IDamagable damagable = hit.GetComponent<IDamagable>();
            
            if (rb != null)
            {
                Vector2 explosionDirection = hit.transform.position - (Vector3) explosionCenter;
                float distance = explosionDirection.magnitude;

                if (distance > 0)
                {
                    explosionDirection.Normalize();
                    float finalExplosionForce = explosionForce;
                    finalExplosionForce += rb.gameObject.layer == LayerMask.NameToLayer("Bomb") ? addBombForce : 0;
                    float force = Mathf.Lerp(finalExplosionForce, 0, distance / explosionRadius);
                    rb.AddForce(explosionDirection * force);

                    //rotate
                    float randomTorque = Random.Range(-explosionForce, explosionForce) * 0.1f;
                    rb.AddTorque(randomTorque, ForceMode2D.Impulse);
                }
            }

            if(damagable != null)
            {
                damagable.TakeDamage(explosionDamage);
            }
        }
    }
    public bool IsExploded() => hasExploded;
    public void SetExtinguished(bool extinguished)
    {
        isExtinguished = extinguished;
        animator.SetBool("IsExtinguished", isExtinguished);
    }

    void ReturnToPool()
    {
        hasExploded = false;
        BombPool.Instance.ReturnBomb(gameObject);
    }

    public void AddForce(Vector2 force)
    {
        rb.AddForce(force);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 explosionCenter = (Vector2)transform.position + explosionOffset;
        Gizmos.DrawWireSphere(explosionCenter, explosionRadius);
    }
}
