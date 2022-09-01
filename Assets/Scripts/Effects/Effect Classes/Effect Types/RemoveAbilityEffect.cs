using UnityEngine;

[CreateAssetMenu(fileName = "New Remove Ability Effect", menuName = "Effects/Effect/RemoveAbility")]
public class RemoveAbilityEffect : Effect
{
    [Header("REMOVE ABILITY EFFECT")]

    [SerializeField] private CardAbility removeAbility;
    [SerializeField] private bool removeAllAbilities;
    [SerializeField] private bool removeAllButNegativeAbilities;
    [SerializeField] private bool removePositiveAbilities;
    [SerializeField] private bool removeNegativeAbilities;

    public CardAbility RemoveAbility { get => removeAbility; }
    public bool RemoveAllAbilities { get => removeAllAbilities; }
    public bool RemoveAllButNegativeAbilities { get => removeAllButNegativeAbilities; }
    public bool RemovePositiveAbilities { get => removePositiveAbilities; }
    public bool RemoveNegativeAbilities { get => removeNegativeAbilities; }

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        RemoveAbilityEffect removeAbilityEffect = effect as RemoveAbilityEffect;
        removeAbility = removeAbilityEffect.RemoveAbility;
        removeAllAbilities = removeAbilityEffect.RemoveAllAbilities;
        removeAllButNegativeAbilities = removeAbilityEffect.RemoveAllButNegativeAbilities;
        removePositiveAbilities = removeAbilityEffect.RemovePositiveAbilities;
        removeNegativeAbilities = removeAbilityEffect.RemoveNegativeAbilities;
    }
}
