using UnityEngine;

[CreateAssetMenu(fileName = "New Give Ability Effect", menuName = "Effects/Effect/GiveAbility")]
public class GiveAbilityEffect : Effect
{
    public CardAbility CardAbility;
    public GiveAbilityType Type;

    public enum GiveAbilityType
    {
        Default,
        RandomPositiveKeyword,
        SavedTarget_PositiveKeywords
    }

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        GiveAbilityEffect gae = effect as GiveAbilityEffect;
        CardAbility = gae.CardAbility;
        Type = gae.Type;
    }
}
