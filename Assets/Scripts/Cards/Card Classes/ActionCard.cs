using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action Card", menuName = "Cards/Action/Action")]
public class ActionCard : Card
{
    [TextArea] [SerializeField] private string effectDescription;
    [SerializeField] private List<EffectGroup> effectGroupList;
    [SerializeField] private List<CardAbility> linkedAbilities;

    public string EffectDescription
    {
        get => effectDescription;
        set => effectDescription = value;
    }
    public List<EffectGroup> EffectGroupList
    {
        get => effectGroupList;
        set => effectGroupList = value;
    }
    public List<CardAbility> LinkedAbilities
    {
        get => linkedAbilities;
        set => linkedAbilities = value;
    }

    public override void LoadCard(Card card)
    {
        base.LoadCard(card);
        ActionCard ac = card as ActionCard;
        effectDescription = ac.EffectDescription;
        effectGroupList = ac.EffectGroupList;
        linkedAbilities = ac.LinkedAbilities;
    }
}
