using UnityEngine;

[CreateAssetMenu(fileName = "New Give Ability Effect", menuName = "Effects/Effect/GiveAbility")]
public class GiveAbilityEffect : Effect
{
    [Header("GIVE ABILITY EFFECT")]
    public CardAbility CardAbility;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        GiveAbilityEffect gae = effect as GiveAbilityEffect;
        CardAbility = gae.CardAbility;
    }
}
