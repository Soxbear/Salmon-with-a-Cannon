using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public GameObject Projectile;
    public float ShotsPerSecond;
    public float Recoil;
    public bool Automatic;

    public Transform ShootPoint;
    public float WallIntersectCastLength;

    public override void OnPrimary()
    {
        if ((PrimaryReloading) || Automatic)
            return;

        if (ShootPoint.rotation.eulerAngles.y == 180f) {
            Instantiate(Projectile, ShootPoint.position, Quaternion.Euler(0f, 0f, -ShootPoint.eulerAngles.z + 180f));            
        }
        else
            Instantiate(Projectile, ShootPoint.position, ShootPoint.rotation);

        ApplyRecoil(-Recoil);

        ReloadPrimary(1/ShotsPerSecond);
    }

    void Update() {
        if (!Automatic)
            return;

        if (Primary && !PrimaryReloading) {
            //Transform point = Physics2D.Raycast(ShootPoint.position, ShootPoint.right, WallIntersectCastLength, )
            if (ShootPoint.rotation.eulerAngles.y == 180f) {
                Instantiate(Projectile, ShootPoint.position, Quaternion.Euler(0f, 0f, -ShootPoint.eulerAngles.z + 180f));            
            }
            else
                Instantiate(Projectile, ShootPoint.position, ShootPoint.rotation);

            ApplyRecoil(-Recoil);

            ReloadPrimary(1/ShotsPerSecond);
        }
    }
}
