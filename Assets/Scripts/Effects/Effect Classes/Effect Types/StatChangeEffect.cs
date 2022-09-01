using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Change Effect", menuName = "Effects/Effect/StatChange")]
public class StatChangeEffect : Effect
{
    [Header("POWER CHANGE")]
    [Range(-5, 5)]
    public int PowerChange;
    public bool DoublePower;
    public bool SetPowerZero;
    public bool PowerIsDerived;
    public DerivedValueType DerivedPowerType;

    [Header("HEALTH CHANGE")]
    [Range(0, 5)]
    public int HealthChange;
    public bool DoubleHealth;
    public bool HealthIsDerived;
    public DerivedValueType DerivedHealthType;

    [Header("RESET STATS")]
    public bool ResetStats;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        StatChangeEffect sce = effect as StatChangeEffect;
        PowerChange = sce.PowerChange;
        DoublePower = sce.DoublePower;
        SetPowerZero = sce.SetPowerZero;
        PowerIsDerived = sce.PowerIsDerived;
        DerivedPowerType = sce.DerivedPowerType;

        HealthChange = sce.HealthChange;
        DoubleHealth = sce.DoubleHealth;
        HealthIsDerived = sce.HealthIsDerived;
        DerivedHealthType = sce.DerivedHealthType;

        ResetStats = sce.ResetStats;
    }
}
