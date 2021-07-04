using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] private Bar HealthBar;

    private CharacterController Controller;
    private Animator Anim;

    float hor;
    float ver;
    [SerializeField] float Speed = 5;
    Vector3 desireMoveDirection;
    [SerializeField] bool blockRotationPlayer;
    [SerializeField] private float desiredRotationSpeed;
    [SerializeField] private float MovementSpeed;
    [SerializeField] float allowPlayerRotation = 0;

    [SerializeField] Camera Cam;

    [SerializeField] ParticleSystem dashEffect;

    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;
    bool IsGrounded;
    Vector3 Velocity;

    public bool IsAttacking { set; get; }
    public int Combo { get; set; } = 0;
    private bool ChainAttack;
    public bool Alive { get; set; } = true;
    private float MaxHealthPoints = 10;
    private float HealthPoints;

    bool isDashing;
    float dashingTime = 1f;
    float dashingSpeed = 7f;
    float dashCounter;

    public Damage AttackStats => new Damage(this, 2);
    public Text enemyCounter;
    private int killedEnemies;
    public int EnemiesCounter 
    {
        get 
        {
            return killedEnemies;
        }
        set
        {
            killedEnemies = value;
            enemyCounter.text = value.ToString();
        }
    }

    void Start()
    {

        HealthPoints = MaxHealthPoints;
        Cam = Camera.main;
        Controller = GetComponent<CharacterController>();
        Anim = GetComponent<Animator>();
        HealthBar.Initialize(HealthPoints, MaxHealthPoints);
        dashEffect.enableEmission = false;
        //WeaponColliderOff();

    }
    Vector3 dashingDirection;
    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if (IsGrounded && Velocity.y < 0)
        {
            Velocity.y = -2f;
        }

        if (Alive && !isDashing)
        {
                if (Input.GetButtonDown("Fire1"))
                {
                    HandleAttack();
                }

                if (Input.GetButtonDown("Jump"))
                {
                    Anim.SetBool("Dash", true);
                    if (IsAttacking)
                    {
                        ComboReset();
                    }
                }

                if (!IsAttacking)
                {
                    InputMagnitude();
                }
        }

        Velocity.y += Physics.gravity.y * Time.deltaTime;
        Controller.Move(Velocity * Time.deltaTime);
    }

    public void StartDashing()
    {
        isDashing = true;
        dashEffect.enableEmission = true;
        dashCounter = 0f;
        SetDirection();
    }

    public void Dashing()
    {
        Controller.Move(transform.forward * dashingSpeed * Time.deltaTime);
        dashCounter += Time.deltaTime;
    }
    public void EndDashing()
    {
        isDashing = false;
        dashEffect.enableEmission = false;
        Anim.SetBool("Dash", false);
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position + new Vector3(0, .5f, 0), CalculateDesiredMoveDirection(hor,ver) * 5,Color.red,Time.deltaTime);
        //Debug.Log(transform.position + new Vector3(0, 1, 0));


        //float i = Vector3.Dot(gameObject.transform.forward, transform.InverseTransformPoint(friend.transform.position));

        //var friend = FindObjectOfType<FriendAI>();
        //Vector3 targetDir = friend.transform.position - transform.position;
        // float angle = Vector3.Angle(targetDir, transform.forward);
        // Debug.Log("angle: " + angle);
        // if (angle < 5.0f)
        // {
        //     print("close");

        // }


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
            Movement();
        }

        Anim.SetFloat("Movement", MovementSpeed, 0f, Time.deltaTime);
    }

    public void Movement()
    {
        desireMoveDirection = CalculateDesiredMoveDirection(hor,ver);

        if (!blockRotationPlayer)
        { 
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(desireMoveDirection), desiredRotationSpeed);
        }
        //Vector3(x, gravity, z)
        Controller.Move(desireMoveDirection *  Speed * Time.deltaTime);

    }

    public void SetDirection()
    {

        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        float treshold = 0.1f;
        
        if (Mathf.Abs(hor) >= treshold || Mathf.Abs(ver) >= treshold)
        {
            desireMoveDirection = CalculateDesiredMoveDirection(hor, ver);

            //transform.LookAt(desireMoveDirection);
            transform.rotation = Quaternion.LookRotation(desireMoveDirection);
        }
    }

    public void FindNearestEnemyToPlayerDirection()
    {
        Vector3 playerPosition = gameObject.transform.position;
        float distance = 5f;

        EnemyAI target = null;
        float lastAngle = float.MaxValue;

        float treshold = 0.15f;

        if (Mathf.Abs(hor) >= treshold || Mathf.Abs(ver) >= treshold)
        {
            hor = Input.GetAxis("Horizontal");
            ver = Input.GetAxis("Vertical");

            desireMoveDirection = CalculateDesiredMoveDirection(hor, ver);
        }
        else
        {
            desireMoveDirection = transform.forward;
        }

        EnemyAI[] allEnemyList = FindObjectsOfType<EnemyAI>();
        var closeEnemies = allEnemyList.Where(x => Vector3.Distance(playerPosition, x.transform.position) < distance);

        foreach (var enemy in closeEnemies)
        {
            Vector3 dir = enemy.transform.position - transform.position;
            float ang = Vector3.Angle(dir, desireMoveDirection);

            if (ang < 40)
            {
                if (target == null || ang < lastAngle)
                {
                    lastAngle = ang;
                    target = enemy;
                }
            }
        }

        if (target == null)
        {
            transform.rotation = Quaternion.LookRotation(desireMoveDirection);
        }
        else
        {
            //transform.rotation = Quaternion.LookRotation(target.transform.position);
            transform.LookAt(target.transform.position);
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
            Controller.Move(Vector3.zero);
            //Rigidbody.velocity = Vector3.zero;
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

