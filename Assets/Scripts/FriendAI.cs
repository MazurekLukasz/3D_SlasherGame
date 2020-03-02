using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendAI : CharacterAI
{
    private GameObject TargetToBeat;
    [SerializeField] bool Fight;

    [SerializeField] private Bar HealthBar;
    [SerializeField] float EnemyDistance = 10;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        HealthBar.Initialize(HealthPoints, MaxHealthPoints);
    }

    // Update is called once per frame
    new void Update()
    {
        if (!LookForFight())
        {
            base.Update();
            //CheckForEnemies();

        }
        else
        {
            if (!Fight)
            {
                ChooseEnemy();
            }
            BattleWithTarget();
            
        }
    }

    void ChooseEnemy()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        
        // find nearest
        float dist = Vector3.Distance(this.transform.position, Enemies[0].transform.position);
        TargetToBeat = Enemies[0];

        foreach (GameObject enemy in Enemies)
        {
            if (Vector3.Distance(this.transform.position, enemy.transform.position) < dist)
            {
                TargetToBeat = enemy;
                dist = Vector3.Distance(this.transform.position, enemy.transform.position);
            }
        }

        Fight = true;
    }

    public bool LookForFight()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in Enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= EnemyDistance && enemy.GetComponent<EnemyAI>().Alive)
            {
                Agent.stoppingDistance = 2f;
                return true;
            }
        }
        Agent.stoppingDistance = 5f;
        TargetToBeat = null;
        return false;
    }

    public void BattleWithTarget()
    {
        if (!TargetToBeat.GetComponent<EnemyAI>().Alive)
        {
            Fight = false;
        }
        else
        {
            float distance = Vector3.Distance(this.transform.position, TargetToBeat.transform.position);
            if (distance >= 2f)
            {
                Agent.SetDestination(TargetToBeat.transform.position);
            }
            else
            {
                MeeleAttack();
            }
        }
        Anim.SetFloat("Movement", Agent.velocity.normalized.magnitude);
    }

    void MeeleAttack()
    {
        Agent.velocity = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(TargetToBeat.transform.position - transform.position);
        Anim.SetTrigger("Attack0");
    }

    public void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other, "EnemyWeapon");
        HealthBar.CurrentValue = HealthPoints;
    }
}
