using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IDamagable : MonoBehaviour 
{
    [SerializeField] protected int maxHealth = 20;
    [SerializeField] protected int currentHealth = 20;
    protected Animator animator;
    [SerializeField] protected bool isDead = false;
    [SerializeField] protected float damagePreventTime = 0.15f;

    private bool isPreventDamage = false;

    protected void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }
    public void TakeDamage(int damage)
    {
        if (!isDead && !isPreventDamage)
        {
            animator.SetBool("TakeDamage",true);
            currentHealth -= damage;
            isPreventDamage = true;
            StartCoroutine(WaitToDamagable());
            if(currentHealth <= 0)
            {
                OnDead();
                isDead = true;
                animator.SetBool("IsDead",true);
            }
        }
    }

    IEnumerator WaitToDamagable()
    {
        yield return new WaitForSeconds(damagePreventTime);
        animator.SetBool("TakeDamage", false);
        isPreventDamage = false;
    }

    public abstract void OnDead();
}
