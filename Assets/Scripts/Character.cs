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
    private Vector3 moveVector;
    [SerializeField] Camera Cam;

    [SerializeField] ParticleSystem dashEffect;

    public bool IsAttacking { set; get; }
    public int Combo { get; set; } = 0;
    private bool ChainAttack;
    public bool Alive { get; set; } = true;
    private float MaxHealthPoints = 10;
    private float HealthPoints;

    bool isDashing;
    float dashingTime = 1f;
    float dashingSpeed = 10f;
    float dashCounter;

    void Start()
    {
        HealthPoints = MaxHealthPoints;
        Cam = Camera.main;
        Controller = GetComponent<CharacterController>();
        Anim = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        HealthBar.Initialize(HealthPoints, MaxHealthPoints);

        dashEffect.enableEmission = false;
        //WeaponColliderOff();

    }
    Vector3 dashingDirection;
    // Update is called once per frame
    void Update()
    {
        if (Alive)
        {
            if (!isDashing)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    HandleAttack();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (!IsAttacking)
                    {
                        SetDirection();
                        isDashing = true;
                        dashEffect.enableEmission = true;
                        dashCounter = 0f;
                        Anim.SetBool("Dash", true);
                    }
                }

                if (!IsAttacking)
                {
                    InputMagnitude();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            if (dashCounter >= dashingTime)
            {
                isDashing = false;
                dashEffect.enableEmission = false;
                Anim.SetBool("Dash", false);
            }
            else
            {
                Controller.Move(transform.forward * dashingSpeed * Time.deltaTime);
                dashCounter += Time.deltaTime;
            }

        }
    }

    void InputMagnitude()
    {

        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        //Anim.SetFloat("InputZ", ver, 0f, Time.deltaTime /** 2f*/);
        //Anim.SetFloat("InputX", hor, 0f, Time.deltaTime /** 2f*/);

        MovementSpeed = new Vector2(hor, ver).sqrMagnitude;

        if (MovementSpeed > allowPlayerRotation)
        {
            //Anim.SetFloat("Movement", MovementSpeed, 0f, Time.deltaTime);
            Movement();
        }
        //else if (MovementSpeed < allowPlayerRotation)
        //{
        //    Anim.SetFloat("Movement", MovementSpeed, 0f, Time.deltaTime);
        //}

        Anim.SetFloat("Movement", MovementSpeed, 0f, Time.deltaTime);
    }

    public void Movement()
    {
        desireMoveDirection = CalculateDesiredMoveDirection(hor,ver);

        if (!blockRotationPlayer)
        { 
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(desireMoveDirection), desiredRotationSpeed);
        }

        Controller.Move(desireMoveDirection *  Speed * Time.deltaTime);

    }

    public void SetDirection()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        float treshold = 0.7f;
        
        if (hor >= Mathf.Abs(treshold) || ver >= Mathf.Abs(treshold))
        {
            desireMoveDirection = CalculateDesiredMoveDirection(hor, ver);

            //transform.LookAt(desireMoveDirection);
            transform.rotation = Quaternion.LookRotation(desireMoveDirection);
        }
    }

    private Vector3 CalculateDesiredMoveDirection(float horizontal, float vertical)
    {
        var forward = Cam.transform.forward;
        var right = Cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return (forward * vertical) + (right * horizontal);
    }

    public void HandleAttack()
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

    #region AnimationEvents

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

    #endregion 

    private void OnTriggerEnter(Collider other)
    {
        if (Alive)
        {
            if (other.tag == "EnemyWeapon")
            {
                GetDamage(2f);
                Debug.Log("Hit!!!");
            }
        }
    }

    public void GetDamage(float damage)
    {
        HealthPoints -= damage;
        if (HealthPoints > 0)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            HealthBar.CurrentValue = HealthPoints;
        }

        if (HealthPoints <= 0)
        {
            HealthPoints = 0;
            HealthBar.CurrentValue = 0;
            Anim.SetTrigger("Death");
            Alive = false;
            Controller.enabled = false;
        }
    }
}

