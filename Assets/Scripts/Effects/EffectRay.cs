using System;
using System.Collections;
using UnityEngine;

public class EffectRay : MonoBehaviour
{
    private readonly float minSpeed = 30;
    private readonly float maxSpeed = 100;

    private float distance;
    private float speed;
    private bool isMoving;
    private bool isDestroyed;

    private SpriteRenderer sprite;
    private ParticleSystem particles;
    private GameObject target;
    private Action rayEffect;
    private bool isEffectGroup;

    private EffectManager efMan;

    public static int ActiveRays { get; set; }

    private void Awake()
    {
        efMan = EffectManager.Instance;
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

        //Vector3 newDirection = Vector3.RotateTowards(transform.forward, Target.transform.position, 10, 10);
        //transform.rotation = Quaternion.LookRotation(newRotation);

        distance = Vector2.Distance(transform.position, target.transform.position);
        speed = distance/10;
        if (speed < minSpeed) speed = minSpeed;
        else if (speed > maxSpeed) speed = maxSpeed;
        if (distance > 0)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, target.transform.position, speed);
        }
        else
        {
            AudioManager.Instance.StartStopSound("SFX_DamageRay_End");
            DestroyRay();
        }
    }

    public void SetEffectRay(GameObject target, Action rayEffect, Color rayColor, bool isEffectGroup)
    {
        this.rayEffect = rayEffect;
        this.isEffectGroup = isEffectGroup;
        this.target = target;

        sprite.color = rayColor;
        var main = particles.main;
        main.startColor = sprite.color;
        gameObject.SetActive(true);
    }

    private void DestroyRay()
    {
        isDestroyed = true;
        ActiveRays--;
        Debug.Log("ACTIVE EFFECT RAYS: <" + ActiveRays + ">");

        if (rayEffect == null)
        {
            Debug.LogError("RAY EFFECT IS NULL!");
            return;
        }

        rayEffect();
        if (isEffectGroup) efMan.ActiveEffects--;

        if (ActiveRays < 1)
        {
            if (!isEffectGroup)
                UIManager.Instance.UpdateEndTurnButton(PlayerManager.Instance.IsMyTurn, true);
        }

        GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(DestroyRayNumerator());
    }

    private IEnumerator DestroyRayNumerator()
    {
        while (particles.particleCount > 0) yield return null;
        Destroy(gameObject);
    }
}
