using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action Card", menuName = "Cards/Action")]
public class ActionCard : Card
{
    public string EffectDescription
    {
        get => effectDescription;
        set => effectDescription = value;
    }
    [TextArea]
    [SerializeField] private string effectDescription;
    public List<EffectGroup> EffectGroupList
    {
        get => effectGroupList;
        set => effectGroupList = value;
    }
    [SerializeField] private List<EffectGroup> effectGroupList;

    public override void LoadCard(Card card)
    {
        base.LoadCard(card);
        ActionCard ac = card as ActionCard;
        effectGroupList = ac.EffectGroupList;
    }
}
