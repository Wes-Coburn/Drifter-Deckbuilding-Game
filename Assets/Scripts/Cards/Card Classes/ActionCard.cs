using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action Card", menuName = "Cards/Action")]
public class ActionCard : Card
{
    public List<Effect> EffectGroup
    {
        get => effectGroup;
        set => effectGroup = value;
    }
    [SerializeField] private List<Effect> effectGroup;

    public override void LoadCard(Card card)
    {
        base.LoadCard(card);
        ActionCard ac = card as ActionCard;
        effectGroup = ac.EffectGroup;
    }
}
