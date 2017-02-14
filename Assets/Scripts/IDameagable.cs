using UnityEngine;

public interface IDamagable 
{
    void TakeHit(float damage, RaycastHit hit);

    void TakeDamage(float damage);
}
