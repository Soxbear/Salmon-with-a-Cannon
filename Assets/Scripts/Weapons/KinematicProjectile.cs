using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicProjectile : MonoBehaviour
{
    public int Damage;

    public float Knockback;

    public float InitialSpeed;

    public float penetration;

    public float penetrationSpeedLoss;

    public float DestroyDelay;

    public float MaxLifetime;

    [Header("Layer Masks are only needed for pierce mode")]
    public LayerMask HitMask;
    public LayerMask LevelMask;


    Rigidbody2D Body;

    void Start() {
        Body = GetComponent<Rigidbody2D>();
        Body.AddRelativeForce(new Vector2(InitialSpeed, 0f), ForceMode2D.Impulse);
        Destroy(gameObject, MaxLifetime);
    }

    void OnTriggerEnter2D(Collider2D Hit) {
            Hittable Hittable;

            if (Hit.attachedRigidbody != null) {
                if (Hit.attachedRigidbody.TryGetComponent<Hittable>(out Hittable)) {
                    Hittable.Hit(Damage, Body.linearVelocity.normalized /*Hit.transform.position - transform.position*/, Knockback, HitType.Bullet);
                    penetration -= Hittable.EntryPenetration(transform.position, HitType.Bullet);
                    Body.linearVelocity -= Body.linearVelocity.normalized * penetrationSpeedLoss * Hittable.EntryPenetration(transform.position, HitType.Bullet);
                }
            }

            Destroy(gameObject, DestroyDelay);
    }

    void OnTriggerStay2D(Collider2D hit) {
        Hittable hittable;

        if (hit.attachedRigidbody != null) {
            if (hit.attachedRigidbody.TryGetComponent<Hittable>(out hittable)) {
                penetration -= Body.linearVelocity.magnitude * Time.fixedDeltaTime * hittable.ContinuousPenetration(transform.position, HitType.Bullet);
                Body.linearVelocity -= Body.linearVelocity.normalized * penetrationSpeedLoss * Body.linearVelocity.magnitude * Time.fixedDeltaTime * hittable.ContinuousPenetration(transform.position, HitType.Bullet);
            }
        }
    }
}
