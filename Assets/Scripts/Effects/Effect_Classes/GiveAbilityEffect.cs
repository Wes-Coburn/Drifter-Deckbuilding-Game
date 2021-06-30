using UnityEngine;

[CreateAssetMenu(fileName = "New Give Ability Effect", menuName = "Effects/GiveAbility")]
public class GiveAbilityEffect : Effect
{
    public CardAbility CardAbility;
    public bool IsTemporary;
}
