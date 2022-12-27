using UnityEngine;

[CreateAssetMenu(fileName = "New ModifyNext Effect", menuName = "Effects/Effect/ModifyNextEffect")]
public class ModifyNextEffect : Effect
{
    public ModifierAbility ModifyNextAbility;
    [Tooltip("Give the effect this many times"), Range(0, 5)]
    public int Multiplier;
    [Tooltip("If enabled, the effect will be given an unlimited number of times within the duration")]
    public bool Unlimited;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        ModifyNextEffect mne = effect as ModifyNextEffect;
        ModifyNextAbility = mne.ModifyNextAbility;
        Multiplier = mne.Multiplier;
        Unlimited = mne.Unlimited;
    }
}
