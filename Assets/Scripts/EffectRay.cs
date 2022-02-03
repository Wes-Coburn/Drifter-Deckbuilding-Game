using UnityEngine;
using System;

public class EffectRay : MonoBehaviour
{
    private readonly float minSpeed = 1;
    private readonly float maxSpeed = 10;

    private float distance;
    private float speed;
    private bool isMoving;

    private GameObject target;
    private Action rayEffect;
    private bool isEffectGroup;

    public static int ActiveRays { get; set; }

    private void Awake()
    {
        isMoving = false;
        ActiveRays++;
        Debug.Log("ACTIVE EFFECT RAYS: <" + ActiveRays + ">");
        gameObject.SetActive(false);
    }

    void Update()
    {
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
        speed = distance/20;
        if (speed < minSpeed) speed = minSpeed;
        else if (speed > maxSpeed) speed = maxSpeed;
        if (distance > 0)
        {
            transform.position =
                Vector2.MoveTowards(transform.position, target.transform.position, speed);
        }
        else
        {
            AudioManager.Instance.StartStopSound("SFX_DamageRay_End");
            DestroyRay();
        }
    }

    public void SetEffectRay(GameObject target, Action rayEffect, bool isEffectGroup)
    {
        this.rayEffect = rayEffect;
        this.isEffectGroup = isEffectGroup;
        this.target = target;
        gameObject.SetActive(true);
    }

    void DestroyRay()
    {
        ActiveRays--;
        Debug.Log("ACTIVE EFFECT RAYS: <" + ActiveRays + ">");

        if (rayEffect == null)
        {
            Debug.LogError("RAY EFFECT IS NULL!");
            return;
        }

        rayEffect();
        if (ActiveRays < 1)
        {
            if (isEffectGroup)
            {
                FunctionTimer.Create(() =>
                EffectManager.Instance.FinishEffectGroupList(false), 1f);
            }
            else PlayerManager.Instance.IsMyTurn = true; // TESTING
        }
        Destroy(gameObject);
    }
}
