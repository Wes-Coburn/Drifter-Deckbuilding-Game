using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Effect", menuName = "Effects/Effect/Heal")]
public class HealEffect : Effect
{
    public bool HealFully;

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        HealEffect healEffect = (HealEffect)effect;
        HealFully = healEffect.HealFully;
    }
}
