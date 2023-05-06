using UnityEngine;

[CreateAssetMenu(fileName = "New GiveNextUnit Effect", menuName = "Effects/Effect/GiveNextUnit")]
public class GiveNextUnitEffect : MultiplierEffect
{
    public Effect[] Effects;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        var gnfe = effect as GiveNextUnitEffect;
        Effects = gnfe.Effects;
    }
}
