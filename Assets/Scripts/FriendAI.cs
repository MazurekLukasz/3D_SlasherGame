using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendAI : CharacterAI
{
    [SerializeField] bool Fight;

    [SerializeField] private Bar HealthBar;

    [SerializeField] protected GameObject FriendToFollow;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        HealthBar.Initialize(HealthPoints, MaxHealthPoints);
    }

    // Update is called once per frame
    new void Update()
    {
        if (LookForFight())
        {// Enemies are close....
            if (!Fight)
            {
                ChooseEnemy();
            }
            BattleWithTarget();
        }
        else
        {
            // Follow Trget
            base.Update();
            //CheckForEnemies();
        }
    }

    public bool LookForFight()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in Enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= EnemyDistance && enemy.GetComponent<CharacterAI>().Alive)
            {
                Agent.stoppingDistance = 2f;
                return true;
            }
        }
        Agent.stoppingDistance = 5f;
        Target = FriendToFollow;
        return false;
    }

    void ChooseEnemy()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        
        // find nearest
        float dist = Vector3.Distance(this.transform.position, Enemies[0].transform.position);
        Target = Enemies[0];

        foreach (GameObject enemy in Enemies)
        {
            if (Vector3.Distance(this.transform.position, enemy.transform.position) < dist)
            {
                Target = enemy;
                dist = Vector3.Distance(this.transform.position, enemy.transform.position);
            }
        }

        Fight = true;
    }

    public void BattleWithTarget()
    {
        if (!Target.GetComponent<CharacterAI>().Alive)
        {
            Fight = false;
        }
        else
        {
            float distance = Vector3.Distance(this.transform.position, Target.transform.position);
            if (distance >= 2f)
            {
                Agent.SetDestination(Target.transform.position);
            }
            else
            {
                MeeleAttack();
            }
        }
        Anim.SetFloat("Movement", Agent.velocity.normalized.magnitude);
    }

    public void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other, "EnemyWeapon");
        HealthBar.CurrentValue = HealthPoints;
    }
}
