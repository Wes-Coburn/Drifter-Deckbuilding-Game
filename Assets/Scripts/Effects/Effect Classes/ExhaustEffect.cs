using UnityEngine;

[CreateAssetMenu(fileName = "New Exhaust Effect", menuName = "Effects/Effect/Exhaust")]
public class ExhaustEffect : Effect
{
    public bool SetExhausted;
    [Tooltip("Effects that trigger if the unit is already of the given state: exhausted/refreshed")]
    public EffectGroup IfAlreadyExhaustedEffects;
    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        ExhaustEffect ee = effect as ExhaustEffect;
        SetExhausted = ee.SetExhausted;
        IfAlreadyExhaustedEffects = ee.IfAlreadyExhaustedEffects; // TESTING
    }
}
