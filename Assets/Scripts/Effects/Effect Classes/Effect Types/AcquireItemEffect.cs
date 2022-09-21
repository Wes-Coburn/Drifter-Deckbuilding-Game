using UnityEngine;

[CreateAssetMenu(fileName = "New Acquire Item Effect", menuName = "Effects/Effect/AcquireItem")]
public class AcquireItemEffect : Effect
{
    [SerializeField] private bool includeRareItems;

    public bool IncludeRareItems { get => includeRareItems; }

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        AcquireItemEffect acquireItemEffect = effect as AcquireItemEffect;
        includeRareItems = acquireItemEffect.IncludeRareItems;
    }
}
