using UnityEngine;

[CreateAssetMenu(fileName = "New Delay Effect", menuName = "Effects/Delay")]
public class DelayEffect : Effect
{
    [Header("DELAY VALUE")]
    [Range(0.5f, 3)]
    public float DelayValue;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        DelayEffect de = effect as DelayEffect;
        DelayValue = de.DelayValue;
    }
}
