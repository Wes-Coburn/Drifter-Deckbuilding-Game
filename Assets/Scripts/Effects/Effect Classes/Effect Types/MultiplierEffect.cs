using UnityEngine;

public abstract class MultiplierEffect : Effect
{
    [Tooltip("Give the effect this many times"), Range(0, 5)]
    public int Multiplier;
    [Tooltip("If enabled, the effect will be given an unlimited number of times within the duration")]
    public bool Unlimited;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        var mle = effect as MultiplierEffect;
        Multiplier = mle.Multiplier;
        Unlimited = mle.Unlimited;
    }
}
