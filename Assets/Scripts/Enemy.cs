using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity 
{
    public enum State {Idle, Chasing, Attacking};
    State currentState;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;
    
    Color originalColor;

    float attackDistanceTreshold = 0.5f;
    float timeBetweenAttacks = 1f;
    float damage = 1f;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

	protected override void Start () 
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            // Subscribe to OnDeath event
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
	}

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }
	
	void Update () 
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDistToTarget = (target.position - transform.position).sqrMagnitude;

                if (sqrDistToTarget < Mathf.Pow(attackDistanceTreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
	}

    // Attack Coroutine
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        // Vector3 attackPosition = target.position;
        Vector3 disToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - disToTarget * (myCollisionRadius);

        float attackSpeed = 3f;
        float percent = 0f;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;
            // Enemy's position goes from its original to attack position and parse in interpolation 
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            // skip a frame
            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 disToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - disToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceTreshold/2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
