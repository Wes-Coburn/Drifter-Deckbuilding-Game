using UnityEngine;

[CreateAssetMenu(fileName = "New ChangeCost Effect", menuName = "Effects/Effect/ChangeCost")]
public class ChangeCostEffect : MultiplierEffect
{
    [Header("CHANGE COST EFFECT")]
    [Range(-5, 5)] public int ChangeValue;
    public bool ChangeActionCost;
    public bool ChangeUnitCost;
    public bool ChangeNextCost;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        var cce = effect as ChangeCostEffect;
        ChangeValue = cce.ChangeValue;
        ChangeActionCost = cce.ChangeActionCost;
        ChangeUnitCost = cce.ChangeUnitCost;
        ChangeNextCost = cce.ChangeNextCost;
    }
}
