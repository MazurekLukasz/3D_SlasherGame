using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Bar HealthBar;

    private CharacterController Controller;
    private Animator Anim;
    private Rigidbody Rigidbody;

    float hor;
    float ver;
    [SerializeField] float Speed = 5;
    Vector3 desireMoveDirection;
    [SerializeField] bool blockRotationPlayer;
    [SerializeField] private float desiredRotationSpeed;
    [SerializeField] private float MovementSpeed;
    [SerializeField] float allowPlayerRotation = 0;
    private float verticalVel;
    private Vector3 moveVector;
    [SerializeField] Camera Cam;


    public bool IsAttacking { set; get; }
    public int Combo { get; set; } = 0;
    private bool ChainAttack;
    public bool Alive { get; set; } = true;
    private float MaxHealthPoints = 10;
    private float HealthPoints;

    [SerializeField]  private CapsuleCollider WeaponCollider;

    // Start is called before the first frame update
    void Start()
    {
        HealthPoints = MaxHealthPoints;
        Cam = Camera.main;
        Controller = GetComponent<CharacterController>();
        Anim = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        HealthBar.Initialize(HealthPoints,MaxHealthPoints);
        //WeaponColliderOff();

    }

    // Update is called once per frame
    void Update()
    {
        if (Alive)
        {
            HandleAttack();
           //InputMagnitude();
            if (!IsAttacking)
            {
                InputMagnitude();
            }
        }
    }


    public void Movement()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        var forward = Cam.transform.forward;
        var right = Cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();
        
        desireMoveDirection = (forward * ver) + (right * hor);

        if (!blockRotationPlayer)
        { 
        transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(desireMoveDirection), desiredRotationSpeed);
        }

        //Controller.Move((new Vector3( (forward * ver) * Speed * Time.deltaTime, verticalVel, hor * Speed * Time.deltaTime)));
        Controller.Move(desireMoveDirection *  Speed * Time.deltaTime);

    }

    public void SetDirection()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
        float tmp = 0.7f;
        if (hor >= tmp || ver >= tmp || hor <= -tmp || ver <= -tmp)
        {
            var forward = Cam.transform.forward;
            var right = Cam.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            desireMoveDirection = (forward * ver) + (right * hor);

            //transform.LookAt(desireMoveDirection);
            transform.rotation = Quaternion.LookRotation(desireMoveDirection);
        }
    }

    void InputMagnitude()
    {
        if (!Controller.isGrounded)
        {
            verticalVel -= 2f;
        }
        else
        {
            verticalVel -= 0f;
        }

        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        Anim.SetFloat("InputZ",ver,0f,Time.deltaTime* 2f);
        Anim.SetFloat("InputX", hor, 0f, Time.deltaTime * 2f);

        MovementSpeed = new Vector2(hor, ver).sqrMagnitude;

        if (MovementSpeed > allowPlayerRotation)
        {
            Anim.SetFloat("Movement", MovementSpeed, 0f, Time.deltaTime);
            Movement();
        }
        else if (MovementSpeed < allowPlayerRotation)
        {
            Anim.SetFloat("Movement", MovementSpeed, 0f, Time.deltaTime);
        }
    }

    public void HandleAttack()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            if (IsAttacking)
            {
                ChainAttack = true;
            }
            else if (!IsAttacking)
            {
                IsAttacking = true;
                SwitchComboAttack();

            }
        }
    }

    public void SwitchComboAttack()
    {
        if (Combo == 0)
        {
            Combo = 1;
            Anim.SetTrigger("Attack0");
        }
        else if (Combo == 1)
        {
            Combo = 2;
            Anim.SetTrigger("Attack1");
        }
    }
    public void ComboChain()
    {
        if(ChainAttack)
        {
            ChainAttack = false;
            SwitchComboAttack();
        }
    }
    public void ComboReset()
    {
        IsAttacking = false;
        ChainAttack = false;
        Combo = 0;

        //Anim.ResetTrigger("Attack1");
        //Anim.ResetTrigger("Attack0");
    }


    public void WeaponColliderOn()
    {
        WeaponCollider.enabled = true;
    }
    public void WeaponColliderOff()
    {
        WeaponCollider.enabled = false;
    }

    public void GetDamage()
    {
        if (HealthPoints > 0)
        {
            HealthPoints -= 2;
        }
        else
        {
            Anim.SetTrigger("Death");
        }
    }
}

