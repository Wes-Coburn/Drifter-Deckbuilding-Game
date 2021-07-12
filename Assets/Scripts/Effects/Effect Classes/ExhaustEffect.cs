using UnityEngine;

[CreateAssetMenu(fileName = "New Exhaust Effect", menuName = "Effects/Exhaust")]
public class ExhaustEffect : Effect
{
    public bool SetExhausted;
    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        ExhaustEffect ee = effect as ExhaustEffect;
        SetExhausted = ee.SetExhausted;
    }
}
