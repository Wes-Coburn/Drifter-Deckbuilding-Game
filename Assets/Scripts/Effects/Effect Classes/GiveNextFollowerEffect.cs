using UnityEngine;

[CreateAssetMenu(fileName = "New GiveNextFollower Effect", menuName = "Effects/GiveNextFollower")]
public class GiveNextFollowerEffect : Effect
{
    public Effect Effect;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        GiveNextFollowerEffect gnfe = effect as GiveNextFollowerEffect;
        Effect = gnfe.Effect;
    }
}
