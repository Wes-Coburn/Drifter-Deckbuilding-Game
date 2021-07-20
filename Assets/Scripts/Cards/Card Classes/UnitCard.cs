using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Unit")]
public class UnitCard : Card
{
    public int CurrentPower { get; set; }
    public int CurrentDefense { get; set; }
    public int MaxDefense { get; set; }
    public bool IsExhausted { get; set; }

    public int StartPower { get => power; }
    [SerializeField] private int power;

    public int StartDefense { get => defense; }
    [SerializeField] private int defense;

    public List<CardAbility> StartingAbilities { get => startingAbilities; }
    [SerializeField] private List<CardAbility> startingAbilities;

    public override void LoadCard(Card card)
    {
        base.LoadCard(card);
        UnitCard fc = card as UnitCard;
        power = fc.StartPower;
        defense = fc.StartDefense;
        startingAbilities = fc.StartingAbilities;
    }
}
