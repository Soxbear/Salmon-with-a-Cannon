using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, Hittable, IntertalReferenceUser
{

    [Header("Stats")]
    public int health;
    private Vector2 targetPos;

    private bool navErrorReached;

    private bool canAttack;

    public List<Vector2> inertialReferences {
        get; set;
    }

    [Header("Settings")]
    public int maxHealth;
    public float speed;
    public float acceleration;
    public float maxNavError;
    public float velocityCorrection;
    public float attackRange;
    public bool rotation;
    public float maxRotationChange;
    public float attackCooldown;
    public bool canAttackOnStart;
    public float knockbackMultiplier;
    public Vector2Int dnaRange;
    public Vector2Int techRange;
    public Color32 confettiColor;
    bool disableConfetti;
    public LayerMask playerMask;
    public LayerMask wallMask;

    public SpriteRenderer[] toFlipSprite;
    public Transform[] toFlipObject;

    [Header("References")]
    protected Rigidbody2D body;
    protected Animator animator;

    protected EnemyManager enemyManager;

    public static Player player;

    protected void Knockback(float strength, Vector2 direction) {
        body.AddForce(direction.normalized * strength, ForceMode2D.Impulse);
    }

    protected bool DetectFull(bool setNav = false) {
        if (Physics2D.OverlapCircle(transform.position, attackRange, playerMask)) {
            if (Physics2D.Raycast(transform.position, (Vector2) player.transform.position - (Vector2) transform.position, ((Vector2) player.transform.position - (Vector2) transform.position).magnitude, wallMask).collider == null) {
                if (setNav)
                    Navigate((Vector2) player.transform.position);
                
                return true;
            }
        }

        return false;
    }

    protected bool Detect(bool setNav = true) {
        if (lastFrameDetection && setNav)
            Navigate(player.transform.position);

        return lastFrameDetection;
    }

    public delegate void DetectionChange(float timeSinceLast);

    public DetectionChange onDetect;

    public DetectionChange onDetectLoss;

    protected float timeSinceLastDetectionChange;

    private bool lastFrameDetection;

    protected void Navigate(Vector2 position) {
        targetPos = position;
        navErrorReached = false;
    }

    protected void CancelNavigation()
    {
        navErrorReached = true;
    }

    public OnEnemyDeath OnDeath;

    protected Vector2 moveVector;

    float lastTrueAngle;

    void UpdateInternal() {
        moveVector = targetPos - (Vector2) transform.position;

        if (moveVector.magnitude < maxNavError)
            navErrorReached = true;        

        if (rotation && !navErrorReached) {
            Vector2 rotVector = moveVector;
            
            float desiredAngle = Vector2.SignedAngle(Vector2.right, rotVector);

            float trueAngle = Mathf.MoveTowardsAngle(lastTrueAngle, desiredAngle, maxRotationChange * Time.deltaTime);
            
            body.MoveRotation(trueAngle);

            if (Mathf.Cos(trueAngle * Mathf.Deg2Rad) < -0.01f) {
                foreach (SpriteRenderer sprite in toFlipSprite)
                    sprite.flipY = true;
                
                foreach (Transform obj in toFlipObject)
                    obj.localScale = new Vector3(1, -1, 1);
            }
            else if (Mathf.Cos(trueAngle * Mathf.Deg2Rad) > 0.01f) {
                foreach (SpriteRenderer sprite in toFlipSprite)
                    sprite.flipY = false;
                
                foreach (Transform obj in toFlipObject)
                    obj.localScale = new Vector3(1, 1, 1);
            }

            lastTrueAngle = trueAngle;
        }
    }

    void FixedUpdateInternal() {
        moveVector = targetPos - (Vector2) transform.position;

        if (!(speed < ((body.linearVelocity - this.GetTotalInertialReference()) + (moveVector.normalized * acceleration * Time.fixedDeltaTime)).magnitude) && !navErrorReached) {
            body.AddForce(Vector2.Lerp(moveVector.normalized, moveVector.normalized - (body.linearVelocity.normalized - moveVector.normalized), velocityCorrection).normalized * acceleration);
        }

        bool detect = DetectFull(false);

        if (detect != lastFrameDetection) {
            if (lastFrameDetection)
                onDetectLoss(timeSinceLastDetectionChange);
            else
                onDetect(timeSinceLastDetectionChange);
            
            timeSinceLastDetectionChange = 0;
        }
        else {
            timeSinceLastDetectionChange += Time.fixedDeltaTime;
        }

        lastFrameDetection = detect;
    }

    public bool Hit(int damage, Vector2 direction, float kb, HitType hitType) {
        health -= damage;
        if (health <= 0) {
            if (!disableConfetti) {
                ParticleSystem particles = ((GameObject) Instantiate(Resources.Load("Confetti"), transform.position, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction)))).transform.GetChild(0).GetComponent<ParticleSystem>();
                ParticleSystem.MainModule main = particles.main;
                main.startColor = new ParticleSystem.MinMaxGradient(confettiColor);
            }

            OnDeath(new EnemyDeathInfo(gameObject.name, this, transform.position, UnityEngine.Random.Range(dnaRange.x, dnaRange.y), UnityEngine.Random.Range(techRange.x, techRange.y)));

            Destroy(gameObject);
            return true;
        }
        Knockback(kb * knockbackMultiplier, direction);
        return false;
    }

    public bool HitNoConfetti(int damage, Vector2 direction, float kb, HitType hitType) {
        health -= damage;
        if (health <= 0) {
            OnDeath(new EnemyDeathInfo(gameObject.name, this, transform.position, UnityEngine.Random.Range(dnaRange.x, dnaRange.y), UnityEngine.Random.Range(techRange.x, techRange.y)));

            Destroy(gameObject);
            return true;
        }
        Knockback(kb * knockbackMultiplier, direction);
        return false;
    }

    protected bool RequestAttack() {
        if (canAttack) {
            canAttack = false;
            StartCoroutine(ResetAttack());
            return true;
        }
        return false;
    }

    IEnumerator ResetAttack() {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private bool initialized = false;

    void OnEnable() {
        if (initialized)
            return;

        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        enemyManager = FindFirstObjectByType<EnemyManager>();
        StartCoroutine(UpdateInternalLoop());
        StartCoroutine(FixedUpdateInternalLoop());

        if (canAttackOnStart)
            canAttack = true;
        else
            StartCoroutine(ResetAttack());

        OnDeath += (info) => {
            enemyManager.OnEnemyDeath.Invoke(info);
        };
        enemyManager.RegisterEnemy.Invoke(this);

        inertialReferences = new List<Vector2>();

        targetPos = transform.position;

        initialized = true;
    }

    IEnumerator UpdateInternalLoop() {
        while (true) {
            UpdateInternal();
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator FixedUpdateInternalLoop() {
        while (true) {
            yield return new WaitForFixedUpdate();
            FixedUpdateInternal();
        }
    }
}