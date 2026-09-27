using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class Player : MonoBehaviour, Hittable, IntertalReferenceUser
{
    [Header("Health")]

    [SerializeField]
    private int hp;
    public int health {
        get { return hp; }
        set {
            hp = Mathf.Max(0, Mathf.Min(value, maxHealth));

            UIManager.healthUI.health = hp;

            if (hp <= 0)
                OnDeath();
        }
    }

    [SerializeField]
    protected float regen;

    [SerializeField]
    private int _maxHealth;
    public int maxHealth {
        get {
            return _maxHealth + maxHealthBoost;
        }
        // set {
        //     _maxHealth = value;
        //     UIManager.healthUI.maxHealth = value;
        // }
    }

    [SerializeField]
    protected int maxHealthBoost;



    [Header("Movement")]
    
    protected bool useDefaultMovement = true;

    public float maxSpeed;

    [SerializeField]
    protected float maxSpeedUpgrade;

    public float acceleration;

    [SerializeField]
    protected float accelerationUpgrade;

    public float idleDrag;

    [SerializeField]
    protected float idleDragUpgrade;

    public List<Vector2> inertialReferences {
        get; set;
    }



    [Header("Animation")]

    [HideInInspector]
    Animator animator;

    protected bool useDefaultAnimator = true;
    protected bool useDefaultRotation = true;

    public bool animationControl = true;

    public float maxRotationChange;



    [Header("Controls")]
    protected Action OnDeath;

    protected Action OnPrimary;

    protected bool primary {
        get {
            return (controls != null) ? controls.Player.Fire.IsPressed() : false;
        }
    }
    
    protected Action OnSecondary;

    protected bool secondary {
        get {
            return (controls != null) ? controls.Player.Secondary.IsPressed() : false;
        }
    }

    public bool hasControl = true;

    [HideInInspector]
    public Vector2 moveVector;

    [HideInInspector]
    public Vector2 aimPos;

    protected bool UIOpen {
        set {
            openUI = value;
            if (value)
                controls.Player.Disable();
            else
                controls.Player.Enable();
        }
        get {
            return openUI;
        }
    }
    bool openUI;
    
    public float maxInteractionRange;

    public LayerMask interactableMask;
    
    public InputMaster controls;



    [HideInInspector]
    public Rigidbody2D body;

    [HideInInspector]
    public Transform playerBody;

    // protected UIController UI;

    protected EnemyManager enemyManager;

    public bool Hit(int damage, Vector2 direction, float knockback, HitType hitType) {
        health -= damage;

        if (health <= 0) {
            //Death code
            return true;
        }

        body.AddForce(direction.normalized * knockback, ForceMode2D.Impulse);
        return false;
    }

    public void Heal(int amount) {
        health += amount;
    }

    IEnumerator Regen() {
        while (true) {
            if (regen > 0) {
                yield return new WaitForSeconds(1/regen);
                health++;
            }
            else
                yield return new WaitForFixedUpdate();
        }
    }

    void OnEnable()
    {     
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyManager = FindFirstObjectByType<EnemyManager>();
        controls = new InputMaster(); 
        playerBody = transform.GetChild(1).transform;
        
        UIManager.healthUI.maxHealth = _maxHealth;
        UIManager.healthUI.health = health;

        controls.Enable();
        controls.Player.Movement.performed += ctx => { 
            moveVector = ctx.ReadValue<Vector2>(); 
        };
        controls.Player.Movement.canceled += ctx => { moveVector = ctx.ReadValue<Vector2>(); };
        
        controls.Player.Fire.performed += (Info) => {
            if (Time.timeScale != 0 && !UIOpen)
                OnPrimary();
        };
        controls.Player.Secondary.performed += (Info) => {
            if (Time.timeScale != 0 && !UIOpen)
                OnSecondary();
        };
        controls.Player.Interact.performed += (Info) => {
            if (Time.timeScale == 0)
                return;

            Collider2D interactObject = Physics2D.OverlapCircle(transform.position, maxInteractionRange, interactableMask);

            if (interactObject != null)
                interactObject.GetComponent<Interactable>().Interact();
        };
        controls.Menu.Exit.performed += (Info) => {
            MenuController.Exit();
        };

        OnDeath += () => {

        };

        enemyManager.OnEnemyDeath.AddListener((Info) => {
        });

        inertialReferences = new List<Vector2>();
        
        StartCoroutine(UpdateInternalLoop());
        StartCoroutine(FixedUpdateInternalLoop());

        StartCoroutine(Regen());
    }

    void FixedUpdateInternal()
    {
        if (!useDefaultMovement)
            goto EndDefaultMovement;

        if (!((maxSpeed) < ((body.linearVelocity - this.GetTotalInertialReference()) + (moveVector * (acceleration) * Time.fixedDeltaTime)).magnitude))
            body.AddForce((moveVector * (acceleration)));
        
        if (moveVector == Vector2.zero)
            body.linearDamping = idleDrag;
        else
            body.linearDamping = 1;

        EndDefaultMovement:;
    }

    float lastTrueAngle;

    void UpdateInternal() {
        if (!useDefaultAnimator)
            goto EndDefaultAnimation;

        animator.SetFloat("Speed", moveVector.magnitude * maxSpeed, 0.2f, Time.deltaTime);

        EndDefaultAnimation:


        if (!useDefaultRotation)
            goto EndDefaultRotation;

        if (moveVector.magnitude > 0.05f) {
            Vector2 rotVector = moveVector;

            float desiredAngle = Vector2.SignedAngle(Vector2.right, rotVector);

            float trueAngle = Mathf.MoveTowardsAngle(lastTrueAngle, desiredAngle, maxRotationChange * Time.deltaTime);

            playerBody.localRotation = Quaternion.Euler(0f, 0f, trueAngle);

            if (Mathf.Cos(trueAngle * Mathf.Deg2Rad) < -0.01f) {
                playerBody.GetComponent<SpriteRenderer>().flipY = true;
            }
            else if (Mathf.Cos(trueAngle * Mathf.Deg2Rad) > 0.01f) {
                playerBody.GetComponent<SpriteRenderer>().flipY = false;
            }

            lastTrueAngle = trueAngle;
        }
        
        EndDefaultRotation:
        
        aimPos = (Vector2) Camera.main.ScreenToWorldPoint(controls.Player.AimScreen.ReadValue<Vector2>()) - (Vector2) transform.position;
    }
    
    public Player() {
        Enemy.player = this;
    }

    IEnumerator UpdateInternalLoop() {
        while (true) {
            UpdateInternal();
            yield return null;
        }
    }
    IEnumerator FixedUpdateInternalLoop() {
        while (true) {
            yield return new WaitForFixedUpdate();
            FixedUpdateInternal();
        }
    }
}

public interface WeaponUser {
    Transform weaponPivot {
        get; set;
    }
    Rigidbody2D rb {
        get; set;
    }
    WeaponObject weapon {
        get; set;
    }
    Weapon weaponClass {
        get; set;
    }
}