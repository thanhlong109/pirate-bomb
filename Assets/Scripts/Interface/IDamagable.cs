using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable 
{
    int CurrentHealth { get; }
    bool IsDead { get; }
    void TakeDamage(int damage);
    public void OnDead();
}
