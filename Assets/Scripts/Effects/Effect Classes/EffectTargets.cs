using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Targets", menuName = "Effects/Effect Targets")]
public class EffectTargets : ScriptableObject
{
    [Header("NO TARGETS")]
    public bool NoTargets;

    [Header("TARGET NUMBER")]
    [Range(1, 10)]
    public int TargetNumber;
    public bool VariableNumber;
    public bool AllowZero;

    [Header("TARGETS ALL")]
    public bool TargetsAll;

    [Header("TARGETS LOWEST HEALTH")]
    public bool TargetsLowestHealth;

    [Header("TARGETS STRONGEST")]
    public bool TargetsStrongest;

    [Header("TARGEST WEAKEST")]
    public bool TargetsWeakest;

    [Header("TARGETS HIGHEST COST")]
    public bool TargetsHighestCost;

    [Header("TARGETS LOWEST COST")]
    public bool TargetsLowestCost;

    [Header("TARGETS SELF")]
    public bool TargetsSelf;

    [Header("PLAYER")]
    public bool PlayerHero;
    public bool PlayerUnit;
    public bool PlayerHand;
    public bool PlayerDeck;

    //public bool UnitCard;
    //public bool ActionCard;

    [Header("ENEMY")]
    public bool EnemyHero;
    public bool EnemyUnit;

    [Header("SAVED TARGET")]
    public bool SavedTarget;

    public bool CompareTargets(EffectTargets targets)
    {
        //if (TargetNumber != targets.TargetNumber) return false;
        //if (VariableNumber != targets.VariableNumber) return false;
        if (NoTargets != targets.NoTargets) return false;
        if (TargetsAll != targets.TargetsAll) return false;
        if (TargetsLowestHealth != targets.TargetsLowestHealth) return false;
        if (TargetsStrongest != targets.TargetsStrongest) return false;
        if (TargetsWeakest != targets.TargetsWeakest) return false;
        if (TargetsHighestCost != targets.TargetsHighestCost) return false;
        if (TargetsLowestCost!= targets.TargetsLowestCost) return false;
        if (TargetsSelf != targets.TargetsSelf) return false;
        if (PlayerHero != targets.PlayerHero) return false;
        if (PlayerUnit != targets.PlayerUnit) return false;
        if (PlayerHand != targets.PlayerHand) return false;
        if (PlayerDeck != targets.PlayerDeck) return false;
        //if (UnitCard != targets.UnitCard) return false;
        //if (ActionCard!= targets.ActionCard) return false;
        if (EnemyHero != targets.EnemyHero) return false;
        if (EnemyUnit != targets.EnemyUnit) return false;
        if (SavedTarget != targets.SavedTarget) return false;
        return true;
    }
}
