using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{

    public float seenRadius = 5;
    public float attackDistance = 1f;

    public float hitpoints = 3f;
    public float damage = 2f;
    public float attackDelay = 1;

    public float walkDistance = 3;

    public Image progress;

    NavMeshAgent agent;

    float maxHitpoints;
    float coolDown = 0;
    float animCool = 0;

    int behavior = 0; //0 idle 1 chasing 2 walk

    float chaseTime = 0;

    bool isDying = false;

    Animator anim;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        anim = GetComponent<Animator>();

        maxHitpoints = hitpoints;
    }

    private void OnEnable()
    {
        hitpoints = maxHitpoints;
        progress.fillAmount = 1;
        isDying = false;
        chaseTime = 0;
        behavior = 0;
        coolDown = 0;
        animCool = 0;

        anim.SetTrigger("Spawn");
    }

    void Update()
    {
        if (isDying)
            return;

        if (Vector2.Distance(agent.destination, transform.position) > 0.1f)
        {
            transform.right = agent.destination - transform.position;
        } 
        else
        {
            if (behavior == 2)
            {
                behavior = 0;
                agent.destination = transform.position;
            }
        }

        Vector2 vikingPos = Viking.instance.transform.position;
        float distance = Vector2.Distance(vikingPos, transform.position);

        if (coolDown > 0)
            coolDown -= Time.deltaTime;

        if (chaseTime > 0)
            chaseTime -= Time.deltaTime;

        if (distance > seenRadius)
        {
            if (behavior == 2)
            {
                //TODO: stop walking
                return;
            }

            if (behavior == 0)
            {
                if (Random.value > 0.001f)
                    return;

                Vector2 rndPos = Random.insideUnitCircle * walkDistance;

                agent.destination = transform.position + new Vector3(rndPos.x,rndPos.y, 0);
                behavior = 2;

                return;
            }

            if (chaseTime <= 0)
            {
                behavior = 0;
                return;
            }    
        }

        if (coolDown <= 0 && distance < attackDistance)
        {
            agent.destination = transform.position;
            coolDown = attackDelay;
            animCool = attackDelay / 2f;

            //ATTACK sequence
            anim.SetTrigger("Attack");
            return;
        }


        if (animCool > 0)
        {
            animCool -= Time.deltaTime;
            return;
        }

        if (distance < attackDistance)
            return;

        agent.destination = vikingPos;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, seenRadius);
    }

    public void Hit(float hp)
    {
        if (isDying)
            return;

        if (hitpoints <= 0)
            return;

        behavior = 1;
        chaseTime = 2;

        hitpoints -= hp;

        coolDown = attackDelay;
        animCool = attackDelay / 2f;

        progress.fillAmount = hitpoints / maxHitpoints;

        if (hitpoints <= 0)
        {
            Die();
        } 
        else
        {
            anim.SetTrigger("Hit");
        }
    }
    public void Die()
    {
        anim.ResetTrigger("Spawn");
        anim.SetTrigger("Die");
        isDying = true;

        agent.destination = transform.position;

        Invoke(nameof(OnDie), 1.5f);
    }

    void OnDie()
    {
        gameObject.SetActive(false);
    }

    public void  MakeHit()
    {
        Vector2 vikingPos = Viking.instance.transform.position;
        float distance = Vector2.Distance(vikingPos, transform.position);

        if (distance > attackDistance)
            return;

        Viking.instance.Hit(damage);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Arrow arrow))
            return;

        arrow.gameObject.SetActive(false);

        Hit(arrow.hitpoints);
    }
}
