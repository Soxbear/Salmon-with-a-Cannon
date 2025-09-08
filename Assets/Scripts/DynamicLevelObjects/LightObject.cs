using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Soxbear.Channels;

public class LightObject : MonoBehaviour, Hittable
{   
    const float blinkSpeed = 14f;

    public ChannelReadWrite<bool> setState;

    public ChannelReadWrite<bool> toggleState;

    [SerializeField]
    private bool on;

    [SerializeField]
    private int health;

    [SerializeField]
    private int maxHealth;

    [SerializeField]
    private Light2D[] lights;

    private float[] lightIntensity;

    [SerializeField]
    private Sprite onSprite;
    [SerializeField]
    private Sprite offSprite;

    [SerializeField]
    private Material onMaterial;
    [SerializeField]
    private Material offMaterial;

    [SerializeField]
    private GameObject destroyedLight;

    private SpriteRenderer spriteRend;

    public bool Hit(int damage, Vector2 direction, float knockback, HitType hitType) {
        if (hitType == HitType.Bullet || hitType == HitType.Explosion || hitType == HitType.Melee || hitType == HitType.Electric || hitType == HitType.Sharapnel) {
            health -= damage;
            if (health <= 0) {
                Instantiate(destroyedLight, transform.position, transform.rotation);
                setState.onUpdate -= OnSetStateUpdate;
                toggleState.onUpdate -= OnToggleStateUpdate;
                Destroy(gameObject);
            }
            LightState();
        }

        return false;
    }

    void Start() {
        lightIntensity = new float[lights.Length];

        setState.onUpdate += OnSetStateUpdate;
        toggleState.onUpdate += OnToggleStateUpdate;

        spriteRend = GetComponent<SpriteRenderer>();

        for (int i = 0; i < lights.Length; i++) {
            lightIntensity[i] = lights[i].intensity;
        }

        LightState();
    }

    void OnSetStateUpdate(bool value) {
        on = value;
            LightState();
    }

    void OnToggleStateUpdate(bool value) {
        if (value)
            on = !on;
        LightState();
    }

    void LightState() {
        foreach (Light2D light in lights) {
            light.enabled = on;
        }
        spriteRend.sprite = (on) ? onSprite : offSprite;
        spriteRend.material = (on) ? onMaterial : offMaterial;
    }

    void Update() {
        if (Mathf.PerlinNoise1D(Time.time * blinkSpeed) >= ((float) health / (float) maxHealth)) {
            for (int i = 0; i < lights.Length; i++) {
                lights[i].intensity = lightIntensity[i] * 0.85f;
            }
        }
        else {
            for (int i = 0; i < lights.Length; i++) {
                lights[i].intensity = lightIntensity[i];
            }
        }
    }
}
