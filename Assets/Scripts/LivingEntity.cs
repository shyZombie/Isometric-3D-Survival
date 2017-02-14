using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that stores identical properties for all the living organisms, 
// such as playr, enemies, animals, etc.  
public class LivingEntity : MonoBehaviour, IDamagable 
{
    public float startingHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        // TODO: Do something with a hit var

        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    protected void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }

        GameObject.Destroy(gameObject);
    }
}
