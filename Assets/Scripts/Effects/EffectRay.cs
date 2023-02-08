using System;
using System.Collections;
using UnityEngine;

public class EffectRay : MonoBehaviour
{
    private float minSpeed = 40;
    private float maxSpeed = 90;

    private float distance;
    private float speed;
    private bool isMoving;
    private bool isDestroyed;

    private SpriteRenderer sprite;
    private ParticleSystem particles;
    private GameObject target;
    private Action rayEffect;
    private EffectRayType effectRayType;

    public static int ActiveRays { get; set; }

    private void Awake()
    {
        isMoving = false;
        isDestroyed = false;
        sprite = GetComponent<SpriteRenderer>();
        particles = GetComponent<ParticleSystem>();
        ActiveRays++;
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (isDestroyed) return;

        if (target == null)
        {
            if (isMoving)
            {
                Debug.LogError("TARGET IS NULL!");
                DestroyRay();
            }
            return;
        }

        isMoving = true;
        distance = Vector2.Distance(transform.position, target.transform.position);

        //speed = distance/20;
        speed = 15000 / distance;
        if (speed < minSpeed) speed = minSpeed;
        else if (speed > maxSpeed) speed = maxSpeed;

        if (distance > 50)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);

            Vector3 targ = target.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x -= objectPos.x;
            targ.y -= objectPos.y;

            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            angle -= 90;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else
        {
            AudioManager.Instance.StartStopSound("SFX_DamageRay_End");
            DestroyRay();
        }
    }

    public enum EffectRayType
    {
        Default,
        EffectGroup,
        RangedAttack
    }
    public void SetEffectRay(GameObject target, Action rayEffect, Color rayColor, EffectRayType effectRayType)
    {
        if (target == null)
        {
            Debug.LogError("TARGET IS NULL!");
            DestroyRay();
            return;
        }

        this.rayEffect = rayEffect;
        this.effectRayType = effectRayType;
        this.target = target;

        sprite.color = rayColor;
        var main = particles.main;
        main.startColor = sprite.color;
        gameObject.SetActive(true);

        if (effectRayType is EffectRayType.RangedAttack)
        {
            AnimationManager.Instance.ChangeAnimationState(gameObject, "Ranged_Attack");
            //maxSpeed /= 2;
            //minSpeed /= 2;
        }
    }

    private void DestroyRay()
    {
        isDestroyed = true;
        ActiveRays--;
        //Debug.Log("ACTIVE EFFECT RAYS: <" + ActiveRays + ">");

        if (rayEffect == null)
        {
            Debug.LogError("RAY EFFECT IS NULL!");
            return;
        }

        rayEffect();
        if (effectRayType is EffectRayType.EffectGroup) ManagerHandler.EF_MAN.ActiveEffects--;
        else if (ActiveRays < 1) UIManager.Instance.UpdateEndTurnButton(true);

        GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(DestroyRayNumerator());
    }

    private IEnumerator DestroyRayNumerator()
    {
        while (particles.particleCount > 0) yield return null;
        Destroy(gameObject);
    }
}
