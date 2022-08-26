using UnityEngine;

[CreateAssetMenu(fileName = "New GiveNextUnit Effect", menuName = "Effects/Effect/GiveNextUnit")]
public class GiveNextUnitEffect : Effect
{
    public Effect[] Effects;
    [Tooltip("If set to 2+, give the effect this many times"), Range(0, 5)]
    public int Multiplier;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        GiveNextUnitEffect gnfe = effect as GiveNextUnitEffect;
        Effects = gnfe.Effects;
        Multiplier = gnfe.Multiplier;
    }
}
