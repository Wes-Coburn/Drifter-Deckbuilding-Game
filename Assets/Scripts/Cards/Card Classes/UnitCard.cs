using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Unit")]
public class UnitCard : Card
{
    [Header("POWER")]
    [SerializeField] private int power;
    [Header("DEFENSE")]
    [SerializeField] private int defense;
    [SerializeField] private List<CardAbility> startingAbilities;
    [SerializeField] private Sound unitDeathSound;

    // POWER
    public int StartPower { get => power; }
    public int CurrentPower { get; set; }
    // DEFENSE
    public int StartDefense { get => defense; }
    public int CurrentDefense { get; set; }
    public int MaxDefense { get; set; }
    // ABILITIES
    public List<CardAbility> StartingAbilities { get => startingAbilities; }
    // SOUNDS
    public Sound UnitDeathSound { get => unitDeathSound; }
    // EXHAUSTED
    public bool IsExhausted { get; set; }

    public override void LoadCard(Card card)
    {
        base.LoadCard(card);
        UnitCard uc = card as UnitCard;
        power = uc.StartPower;
        defense = uc.StartDefense;
        unitDeathSound = uc.UnitDeathSound;
        startingAbilities = uc.StartingAbilities;
    }
}
