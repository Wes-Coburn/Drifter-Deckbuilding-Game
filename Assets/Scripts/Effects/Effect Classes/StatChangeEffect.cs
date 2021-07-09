using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Change Effect", menuName = "Effects/StatChange")]
public class StatChangeEffect : Effect
{
    public bool IsDefenseChange;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        StatChangeEffect sce = effect as StatChangeEffect;
        IsDefenseChange = sce.IsDefenseChange;
    }
}
