using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piranha : Enemy, ExternalTriggerStay2DUser
{
    [Header("Settings")]
    public int Damage;
    public float DashRange;
    public float DashVelocity;

    [Header("References")]
    Animator BodyAnimator;
    Animator FaceAnimator;

    bool DashHit = false;

    public void ExternalTriggerStay2D(Collider2D Col, int Identifier) {
        Hittable Hittable = Col.GetComponentInParent<Hittable>();
        if (Hittable != null && !DashHit)
        {
            Hittable.Hit(Damage, (Vector2)(transform.position - Col.transform.position), 1f, HitType.Melee);
            DashHit = true;
        }
    }

    void Start()
    {
        BodyAnimator = transform.GetChild(1).GetComponent<Animator>();
        FaceAnimator = transform.GetChild(0).GetComponent<Animator>();

        onDetect += (time) => {
            FaceAnimator.enabled = true;
            FaceAnimator.Play("TeethReveal");
        };
        onDetectLoss += (time) => {
            FaceAnimator.enabled = true;
            FaceAnimator.Play("TeethHide");
        };
    }

    // Update is called once per frame
    void Update()
    {
        BodyAnimator.SetFloat("Speed", body.linearVelocity.magnitude);
    }

    void FixedUpdate() {
        if (Detect() && (player.transform.position - transform.position).magnitude < DashRange && RequestAttack())
        {
            CancelNavigation();
            body.linearVelocity += (Vector2) (player.transform.position - transform.position).normalized * DashVelocity;
            DashHit = false;
        }
    }
}
