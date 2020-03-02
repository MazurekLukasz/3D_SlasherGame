using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dummy : MonoBehaviour
{
    private float HealthPoints = 10;

    private Animator Anim;
    private bool Alive = true;
    Rigidbody rigid;
    [SerializeField] GameObject target;
    float maxDist = 8f;
    float MovementSpeed = 3;
    private CharacterController Controller;
    NavMeshAgent agent;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (target.GetComponent<Character>().Alive)
            {
                FollowPlayer();
            }
        }
            
    }

    public void FollowPlayer()
    {
        float distance = Vector3.Distance(target.transform.position,transform.position);

        if (distance < maxDist)
        {
            agent.SetDestination(target.transform.position);
        }
        if (distance <= agent.stoppingDistance)
        {
            // Attack
            // face target
            FaceTarget();
        }
        Debug.Log("velocity" + rigid.velocity);
        Vector3 vel = rigid.velocity.normalized;
        Debug.Log(vel);
        float MovementS = new Vector2(vel.x, vel.z).sqrMagnitude;
        Debug.Log(MovementS);
        Anim.SetFloat("Movement", MovementS);
    }
    void FaceTarget()
    {
        //Vector3 direction = (target.transform.position - transform.position).normalized;
        //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position), 5f);
    }

    void OnCollisionEnter(Collision col)
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (Alive)
        {
            if (other.tag == "Weapon")
            {
                GetDamage(2f);

            }
        }
    }
    void GetDamage(float dmg)
    {
        HealthPoints -= dmg;
        if (HealthPoints > 0)
        {
            Anim.SetTrigger("Damage");
        }

        if (HealthPoints <= 0)
        {
            HealthPoints = 0;
            Anim.SetTrigger("Death");
            Alive = false;
        }
    }
}
