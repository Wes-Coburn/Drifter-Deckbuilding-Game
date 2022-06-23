using UnityEngine;

[CreateAssetMenu(fileName = "New Static Ability", menuName = "Card Abilities/Static Ability")]
public class StaticAbility : CardAbility
{
    public bool AbilityStacks;
    public Sound GainAbilitySound;
    public Sound LoseAbilitySound;
}
