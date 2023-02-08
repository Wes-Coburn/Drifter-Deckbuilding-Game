using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemHandler : MonoBehaviour
{
    private ParticleSystem particles;
    private GameObject parent;
    private bool usePointerPosition;
    private bool followPosition;

    public enum ParticlesType
    {
        Attack,
        ButtonPress,
        Damage,
        Drag,
        Explosion,
        MouseDrag,
        NewCard,
        Play
    }

    private void Awake() => particles = GetComponent<ParticleSystem>();

    private void Update()
    {
        if (!followPosition || parent == null) return;

        if (usePointerPosition)
        {
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePoint.x, mousePoint.y, 10);
        }
        else
        {
            Vector2 parentPos = parent.transform.position;
            transform.position = new Vector3(parentPos.x, parentPos.y, 10);
        }

    }

    public void StartParticles(GameObject parent, Color startColor,
        ParticleSystem.MinMaxCurve startSize, ParticleSystem.MinMaxCurve startLifetime,
        float stopDelay = 0, bool usePointerPosition = false, bool followPosition = true)
    {
        this.parent = parent;
        this.usePointerPosition = usePointerPosition;
        this.followPosition = followPosition;

        var main = particles.main;
        main.startColor = startColor;
        main.startSize = startSize;
        main.startLifetime = startLifetime;

        if (usePointerPosition)
        {
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePoint.x, mousePoint.y, 10);
        }

        particles.Play();

        if (stopDelay > 0) FunctionTimer.Create(() =>
        {
            if (gameObject != null) StopParticles();
        }, stopDelay);
    }

    public void StopParticles()
    {
        var particleEmission = particles.emission;
        particleEmission.rateOverTime = 0;
        particleEmission.rateOverDistance = 0;
        StartCoroutine(StopParticlesNumertor());
    }

    private IEnumerator StopParticlesNumertor()
    {
        while (gameObject != null &&
            particles.particleCount > 0) yield return null;
        if (gameObject != null) Destroy(gameObject);
    }
}
