using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Effects/Effect/Damage")]
public class DamageEffect : Effect
{
    [Tooltip("If the damage destroys a unit, this effect group list resolves")]
    public List<EffectGroup> IfDestroyedEffects;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        DamageEffect dmgEffect = effect as DamageEffect;
        IfDestroyedEffects = new List<EffectGroup>();
        foreach (EffectGroup eg in dmgEffect.IfDestroyedEffects)
            IfDestroyedEffects.Add(eg);
    }
}
