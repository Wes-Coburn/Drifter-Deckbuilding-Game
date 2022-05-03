using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Unit")]
public class UnitCard : Card
{
    [Header("POWER")]
    [SerializeField] [Range(0, 10)] private int power;
    [Header("HEALTH")]
    [SerializeField] [Range(1, 10)] private int health;
    [Header("ABILITIES")]
    [SerializeField] private List<CardAbility> startingAbilities;
    [SerializeField] private Sound unitDeathSound;

    public int StartPower { get => power; }
    public int StartHealth { get => health; }
    public List<CardAbility> StartingAbilities { get => startingAbilities; }
    public Sound UnitDeathSound { get => unitDeathSound; }

    public int CurrentPower { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    public bool IsExhausted { get; set; }

    public override void LoadCard(Card card)
    {
        base.LoadCard(card);
        UnitCard uc = card as UnitCard;
        power = uc.StartPower;
        health = uc.StartHealth;
        startingAbilities = uc.StartingAbilities;
        unitDeathSound = uc.UnitDeathSound;
    }
}
