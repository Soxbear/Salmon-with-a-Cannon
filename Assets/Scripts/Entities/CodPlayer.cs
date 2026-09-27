using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CodPlayer : Player, WeaponUser
{
    [Header("Stats")]

    Transform AimPivot;

    public WeaponObject weapon {
        get {
            return HeldWeapon;
        }
        set {
            HeldWeapon = value;
            if (transform.GetChild(0).GetChild(0).childCount > 0)
                Destroy(transform.GetChild(0).GetChild(0).GetChild(0));

            Instantiate(HeldWeapon.WeaponGameobject, transform.GetChild(0).GetChild(0));
            weaponClass = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Weapon>();
        }
    }
    public WeaponObject HeldWeapon;

    [HideInInspector]
    public Weapon weaponClass {
        get; set;
    }


    //[Header("References")]
    [HideInInspector]
    public Transform weaponPivot {
        get {
            return AimPivot;
        }
        set {
            AimPivot = value;
        }
    }
    [HideInInspector]
    public Rigidbody2D rb {
        get {
            return body;
        }
        set {
            body = value;
        }
    }

    void Update() {
        float Angle = Vector2.SignedAngle(Vector2.right, aimPos);

        float Subtract = Mathf.Asin(weaponClass.AimOffset / aimPos.magnitude) * Mathf.Rad2Deg;

        if (!float.IsNaN(Angle-Subtract)) {
            AimPivot.Rotate(0f, 0f, (Angle - Subtract) - AimPivot.rotation.eulerAngles.z);
        }
    }

    void Start() {
        AimPivot = transform.GetChild(0);

        weapon = HeldWeapon;

        OnPrimary += () => {
            weaponClass.OnPrimary();
        };
        OnSecondary += () => {
            weaponClass.OnSecondary();
        };
        weaponClass.GetPrimary += () => {
            return primary;
        };
        weaponClass.GetSecondary += () => {
            return secondary;
        };


        OnDeath += () => {
            SceneManager.LoadScene("ded");
        };
    }
}

