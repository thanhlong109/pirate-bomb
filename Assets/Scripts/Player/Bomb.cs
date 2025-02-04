using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Bomb : MonoBehaviour
{

    [SerializeField] private float delay = 3f;
    [SerializeField] private float explosionRadius = 5f;    
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private int explosionDamage = 50;
    [SerializeField] private LayerMask affectedLayers;
    [SerializeField] private Vector2 explosionOffset = Vector2.zero;
    [SerializeField] private float addBombForce = 700f;

    [SerializeField] private bool hasExploded = false;  
    private Animator animator;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ExplodeAfterDelay());
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
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
        Explode();
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;
        animator.SetBool("Explored", true);

        Vector2 explosionCenter = (Vector2)transform.position + explosionOffset;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionCenter, explosionRadius, affectedLayers);

        foreach (Collider2D hit in colliders)
        {
            // Skip the bomb itself
            if (hit.gameObject == gameObject) continue;

            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            
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

            
            //Health health = hit.GetComponent<Health>();
            //if (health != null)
            //{
            //    float damage = Mathf.Lerp(explosionDamage, 0, distance / explosionRadius);
            //    health.TakeDamage((int)damage);
            //}
        }
    }
    public bool IsExploded() => hasExploded;

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
