using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Unit")]
public class UnitCard : Card
{
    [Header("ELITE")]
    [SerializeField] private bool isElite;
    [Header("POWER")]
    [SerializeField] private int power;
    [Header("HEALTH")]
    [SerializeField] private int health;
    [Header("ABILITIES")]
    [SerializeField] private List<CardAbility> startingAbilities;
    [SerializeField] private Sound unitDeathSound;

    // ELITE
    public bool IsElite { get => isElite; }
    // POWER
    public int StartPower { get => power; }
    public int CurrentPower { get; set; }
    // HEALTH
    public int StartHealth { get => health; }
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
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
        isElite = uc.IsElite;
        power = uc.StartPower;
        health = uc.StartHealth;
        unitDeathSound = uc.UnitDeathSound;
        startingAbilities = uc.StartingAbilities;
    }
}
