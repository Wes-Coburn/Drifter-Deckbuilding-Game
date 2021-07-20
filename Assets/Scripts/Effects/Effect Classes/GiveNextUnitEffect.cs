using UnityEngine;

[CreateAssetMenu(fileName = "New GiveNextUnit Effect", menuName = "Effects/GiveNextUnit")]
public class GiveNextUnitEffect : Effect
{
    public Effect Effect;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        GiveNextUnitEffect gnfe = effect as GiveNextUnitEffect;
        Effect = gnfe.Effect;
    }
}
