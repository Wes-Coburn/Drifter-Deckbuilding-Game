using UnityEngine;

[CreateAssetMenu(fileName = "New Remove Ability Effect", menuName = "Effects/Effect/RemoveAbility")]
public class RemoveAbilityEffect : Effect
{
    [Header("REMOVE ABILITY EFFECT")]

    [SerializeField] private bool removeAllAbilities;
    [SerializeField] private bool removeAllButNegativeAbilities;
    [SerializeField] private bool removePositiveAbilities;
    [SerializeField] private bool removeNegativeAbilities;

    public bool RemoveAllAbilities { get => removeAllAbilities; }
    public bool RemoveAllButNegativeAbilities { get => removeAllButNegativeAbilities; }
    public bool RemovePositiveAbilities { get => removePositiveAbilities; }
    public bool RemoveNegativeAbilities { get => removeNegativeAbilities; }
}
