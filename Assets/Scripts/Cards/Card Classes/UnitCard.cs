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
    public List<CardAbility> CurrentAbilities { get; set; }
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
        CurrentPower = power;
        health = uc.StartHealth;
        CurrentHealth = health;
        MaxHealth = health;

        startingAbilities = uc.StartingAbilities;
        CurrentAbilities = new List<CardAbility>();
        foreach (CardAbility abi in startingAbilities)
        {
            // TESTING TESTING TESTING
            CardAbility newAbi = CreateInstance(abi.GetType().Name) as CardAbility;
            newAbi.LoadCardAbility(abi);

            CurrentAbilities.Add(newAbi);
        }

        unitDeathSound = uc.UnitDeathSound;
    }

    public override void CopyCard(Card card)
    {
        base.CopyCard(card);
        UnitCard uc = card as UnitCard;
        power = uc.StartPower;
        CurrentPower = uc.CurrentPower;
        health = uc.StartHealth;
        CurrentHealth = uc.CurrentHealth;
        MaxHealth = uc.MaxHealth;

        startingAbilities = uc.startingAbilities;
        CurrentAbilities = new List<CardAbility>();
        foreach (CardAbility abi in uc.CurrentAbilities)
        {
            // Don't copy ChangeControl abilities
            if (abi is TriggeredAbility tra)
            {
                foreach (EffectGroup eg in tra.EffectGroupList)
                    foreach (Effect e in eg.Effects)
                        if (e is ChangeControlEffect)
                            goto NextAbility;
            }

            // TESTING TESTING TESTING
            CardAbility newAbi = CreateInstance(abi.GetType().Name) as CardAbility;
            newAbi.LoadCardAbility(abi);
            CurrentAbilities.Add(abi);

            NextAbility:;
        }

        unitDeathSound = uc.UnitDeathSound;
    }
}
