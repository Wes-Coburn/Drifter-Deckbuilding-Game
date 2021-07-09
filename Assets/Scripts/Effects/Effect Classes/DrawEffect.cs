using UnityEngine;

[CreateAssetMenu(fileName = "New Draw Effect", menuName = "Effects/Draw")]
public class DrawEffect : Effect
{
    public bool IsDiscardEffect;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        DrawEffect de = effect as DrawEffect;
        IsDiscardEffect = de.IsDiscardEffect;
    }
}
