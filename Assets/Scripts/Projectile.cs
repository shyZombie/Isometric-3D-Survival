using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    float speed = 10f;
    float damage = 1f;

    float lifetime = 3f;
    float skinWidth = 0.1f;

    void Start()
    {
        Destroy(gameObject, lifetime);

        // Array with all the colliders that projectile is intersecting with
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0]);
        }
    }

    //set different speed for the different weapons
    public void SetSpeed(float newSpeed)
    { 
        speed = newSpeed;
    }

	void Update () 
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
	}

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        IDamagable damageableObject = hit.collider.GetComponent<IDamagable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hit);
        }

        GameObject.Destroy(gameObject);
    }

    void OnHitObject(Collider c)
    {
        IDamagable damageableObject = c.GetComponent<IDamagable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }

        GameObject.Destroy(gameObject);
    }
}
