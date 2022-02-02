using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Change Effect", menuName = "Effects/Effect/StatChange")]
public class StatChangeEffect : Effect
{
    [Header("STAT CHANGE EFFECT")]
    public bool IsHealthChange;
    public bool IsDoubled;
    public bool SetToZero;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        StatChangeEffect sce = effect as StatChangeEffect;
        IsHealthChange = sce.IsHealthChange;
        IsDoubled = sce.IsDoubled;
        SetToZero = sce.SetToZero;
    }
}
