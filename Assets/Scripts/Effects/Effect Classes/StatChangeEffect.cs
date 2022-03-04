using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Change Effect", menuName = "Effects/Effect/StatChange")]
public class StatChangeEffect : Effect
{
    [Header("POWER CHANGE")]
    [Range(-5, 5)]
    public int PowerChange;
    public bool DoublePower;
    public bool SetPowerZero;

    [Header("HEALTH CHANGE")]
    [Range(0, 5)]
    public int HealthChange;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        StatChangeEffect sce = effect as StatChangeEffect;
        PowerChange = sce.PowerChange;
        DoublePower = sce.DoublePower;
        SetPowerZero = sce.SetPowerZero;
        HealthChange = sce.HealthChange;
    }
}
