using UnityEngine;

[CreateAssetMenu(fileName = "New ChangeCost Effect", menuName = "Effects/Effect/ChangeCost")]
public class ChangeCostEffect : Effect
{
    [Header("CHANGE COST EFFECT")]
    [Range(-5, 5)] public int ChangeValue;
    public bool ChangeActionCost;
    public bool ChangeUnitCost;
    public bool ChangeNextCost;
    [Tooltip("If set to 2+, give the effect this many times"), Range(0, 5)]
    public int Multiplier;
    [Tooltip("If enabled, the effect will be given an unlimited number of times within the duration")]
    public bool Unlimited;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        ChangeCostEffect cce = effect as ChangeCostEffect;
        ChangeValue = cce.ChangeValue;
        ChangeActionCost = cce.ChangeActionCost;
        ChangeUnitCost = cce.ChangeUnitCost;
        ChangeNextCost = cce.ChangeNextCost;
        Multiplier = cce.Multiplier;
        Unlimited = cce.Unlimited;
    }
}
