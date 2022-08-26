using UnityEngine;

[CreateAssetMenu(fileName = "New Static Ability", menuName = "Card Abilities/Static Ability")]
public class StaticAbility : CardAbility
{
    public bool AbilityStacks;
    public Sound GainAbilitySound;
    public Sound LoseAbilitySound;

    public override void LoadCardAbility(CardAbility cardAbility)
    {
        base.LoadCardAbility(cardAbility);
        StaticAbility staticAbility = cardAbility as StaticAbility;
        AbilityStacks = staticAbility.AbilityStacks;
        GainAbilitySound = staticAbility.GainAbilitySound;
        LoseAbilitySound = staticAbility.LoseAbilitySound;
    }
}
