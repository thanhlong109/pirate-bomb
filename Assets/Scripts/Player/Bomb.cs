using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float delay = 3f;              
    public float explosionRadius = 5f;    
    public float explosionForce = 700f;   
    public int explosionDamage = 50;      
    public LayerMask affectedLayers;
    public Vector2 explosionOffset = Vector2.zero; 

    private bool hasExploded = false;  
    private Animator animator;
    //private Rigidbody2D rb;

    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ExplodeAfterDelay());
        animator = GetComponent<Animator>();
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
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                
                Vector2 explosionDirection = hit.transform.position - (Vector3) explosionCenter;
                float distance = explosionDirection.magnitude;

                if (distance > 0)
                {
                    explosionDirection.Normalize();

                    float force = Mathf.Lerp(explosionForce, 0, distance / explosionRadius);

                    rb.AddForce(explosionDirection * force);

                    // rotate 
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

        
        //Destroy(gameObject);
    }

   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 explosionCenter = (Vector2)transform.position + explosionOffset;
        Gizmos.DrawWireSphere(explosionCenter, explosionRadius);
    }
}
