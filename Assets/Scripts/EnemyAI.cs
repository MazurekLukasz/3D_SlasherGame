using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : CharacterAI
{
    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        if (BlockMovement) return;

        if (Target != null && LookForFight())
        {
            BattleWithTarget();
        }
        else
        {
            base.Update();
        }
  
    }

    private bool LookForFight()
    {
            if (Vector3.Distance(transform.position, Target.transform.position) <= EnemyDistance && (Target.GetComponent<Character>().Alive))
            {
                Agent.stoppingDistance = 2f;
                return true;
            }
        
        Agent.stoppingDistance = 5f;
        //Target = null;
        return false;
    }

    public void BattleWithTarget()
    {
        if (Target.GetComponent<Character>().Alive)
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
        base.OnTriggerEnter(other, "Weapon");
    }
}
