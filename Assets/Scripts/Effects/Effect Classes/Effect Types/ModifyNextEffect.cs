using UnityEngine;

[CreateAssetMenu(fileName = "New ModifyNext Effect", menuName = "Effects/Effect/ModifyNextEffect")]
public class ModifyNextEffect : MultiplierEffect
{
    public ModifierAbility ModifyNextAbility;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        var mne = effect as ModifyNextEffect;
        ModifyNextAbility = mne.ModifyNextAbility;
    }
}
