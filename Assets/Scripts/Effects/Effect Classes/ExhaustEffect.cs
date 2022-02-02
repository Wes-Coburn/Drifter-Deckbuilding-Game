using UnityEngine;

[CreateAssetMenu(fileName = "New Exhaust Effect", menuName = "Effects/Effect/Exhaust")]
public class ExhaustEffect : Effect
{
    [Header("EXHAUSTED EFFECT")]
    public bool SetExhausted;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        ExhaustEffect ee = effect as ExhaustEffect;
        SetExhausted = ee.SetExhausted;
    }
}
