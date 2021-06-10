using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class CharacterAI : MonoBehaviour
{
    [SerializeField] protected GameObject Target;
    protected NavMeshAgent Agent;
    protected Animator Anim;
    protected CharacterController Controller;

    [SerializeField] protected float EnemyDistance = 10;

    protected float MaxHealthPoints = 10;
    protected float HealthPoints;
    public bool Alive { get; set;} = true;

    [SerializeField] protected CapsuleCollider WeaponCollider;
    public bool BlockMovement { set; get; }

    public virtual void Start()
    {
        HealthPoints = MaxHealthPoints;
        Agent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (Alive)
        {
            if (Target != null)
            {
                Agent.SetDestination(Target.transform.position);
            }
            // Update animator
            Anim.SetFloat("Movement", Agent.velocity.normalized.magnitude);
        }
    }

    public void OnCollisionEnter(Collision col)
    {

    }

    public virtual void OnTriggerEnter(Collider other,string WeaponTag)
    {
        if (Alive)
        {
            if (other.tag == WeaponTag)
            {
                GetDamage(2f);
            }
        }
    }

    public void GetDamage(float dmg)
    {
        HealthPoints -= dmg;
        if (HealthPoints > 0)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Anim.SetTrigger("Damage");
        }

        if (HealthPoints <= 0)
        {
            HealthPoints = 0;
            Anim.SetTrigger("Death");
            Alive = false;
            Controller.enabled = false;
            Agent.enabled = false;
            Target = null;
            Destroy(gameObject, 5);
        }
    }

    protected void MeeleAttack()
    {
       // BlockMovement = true;
        Agent.velocity = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(Target.transform.position - transform.position);
        Anim.SetTrigger("Attack0");
    }

    //public void WeaponColliderOn()
    //{
    //    WeaponCollider.enabled = true;
    //}
    //public void WeaponColliderOff()
    //{
    //    WeaponCollider.enabled = false;
    //}

    void ComboReset()
    { }
    void ComboChain()
    { }
}
