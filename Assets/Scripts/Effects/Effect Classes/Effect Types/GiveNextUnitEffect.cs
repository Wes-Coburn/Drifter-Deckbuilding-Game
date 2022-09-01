using UnityEngine;

[CreateAssetMenu(fileName = "New GiveNextUnit Effect", menuName = "Effects/Effect/GiveNextUnit")]
public class GiveNextUnitEffect : Effect
{
    public Effect[] Effects;
    [Tooltip("Give the effect this many times"), Range(0, 5)]
    public int Multiplier;
    [Tooltip("If enabled, the effect will be given an unlimited number of times within the duration")]
    public bool Unlimited;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        GiveNextUnitEffect gnfe = effect as GiveNextUnitEffect;
        Effects = gnfe.Effects;
        Multiplier = gnfe.Multiplier;
        Unlimited = gnfe.Unlimited;
    }
}
