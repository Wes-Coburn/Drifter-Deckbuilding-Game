using UnityEngine;

[CreateAssetMenu(fileName = "New Give Ability Effect", menuName = "Effects/Effect/GiveAbility")]
public class GiveAbilityEffect : Effect
{
    [Header("CARD ABILITY")]
    public CardAbility CardAbility;

    [Header("IF ALREADY HAS EFFECTS")]
    [Tooltip("Effects that trigger if the unit already has the given ability")]
    public EffectGroup IfAlreadyHasEffects;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        GiveAbilityEffect gae = effect as GiveAbilityEffect;
        CardAbility = gae.CardAbility;
        IfAlreadyHasEffects = gae.IfAlreadyHasEffects;
    }
}
