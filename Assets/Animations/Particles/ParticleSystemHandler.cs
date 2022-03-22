using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemHandler : MonoBehaviour
{
    [SerializeField] private Color attackColor;
    [SerializeField] private Color buttonPressColor;
    [SerializeField] private Color damageColor;
    [SerializeField] private Color dragColor;
    [SerializeField] private Color playColor;

    private ParticleSystem particles;
    private GameObject parent;

    public enum ParticlesType
    {
        Attack,
        ButtonPress,
        Damage,
        Drag,
        Play
    }

    private void Awake() => particles = GetComponent<ParticleSystem>();

    private void Update()
    {
        if (parent == null) return;
        Vector2 parentPos = parent.transform.position;
        transform.position = new Vector3(parentPos.x, parentPos.y, 10);
    }

    public void StartParticles(GameObject parent, ParticlesType particlesType, float stopDelay = 0)
    {
        this.parent = parent;
        Color startColor;

        switch (particlesType)
        {  
            case ParticlesType.Attack:
                startColor = attackColor;
                break;
            case ParticlesType.ButtonPress:
                startColor = buttonPressColor;
                break;
            case ParticlesType.Damage:
                startColor = damageColor;
                break;
            case ParticlesType.Drag:
                startColor = dragColor;
                break;
            case ParticlesType.Play:
                startColor = playColor;
                break;
            default:
                Debug.LogError("PARTICLES TYPE NOT FOUND!");
                return;
        }

        var main = particles.main;
        main.startColor = startColor;

        particles.Play(); // TESTING

        if (stopDelay > 0) FunctionTimer.Create(() =>
        {
            if (gameObject != null) StopParticles();
        }, stopDelay);
    }

    public void StopParticles()
    {
        var particleEmission = particles.emission;
        particleEmission.rateOverTime = 0;
        StartCoroutine(StopParticlesNumertor());
    }

    private IEnumerator StopParticlesNumertor()
    {
        while (gameObject != null &&
            particles.particleCount > 0) yield return null;
        if (gameObject != null) Destroy(gameObject);
    }
}
