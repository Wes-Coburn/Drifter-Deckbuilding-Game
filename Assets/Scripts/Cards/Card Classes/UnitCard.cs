using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Unit")]
public class UnitCard : Card
{
    [Header("POWER")]
    [SerializeField][Range(0, 10)] private int power;
    [Header("HEALTH")]
    [SerializeField][Range(1, 10)] private int health;
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
    public List<CardAbility> CurrentAbilities { get; set; }

    public override void LoadCard(Card card)
    {
        base.LoadCard(card);
        var uc = card as UnitCard;
        power = uc.StartPower;
        CurrentPower = power;
        health = uc.StartHealth;
        CurrentHealth = health;
        MaxHealth = health;
        unitDeathSound = uc.UnitDeathSound;

        startingAbilities = uc.StartingAbilities;
        CurrentAbilities = new();

        foreach (var abi in startingAbilities)
        {
            if (abi == null)
            {
                Debug.LogError($"EMPTY ABILITY! <{card.CardName}>");
                continue;
            }

            var newAbi = CreateInstance(abi.GetType().Name) as CardAbility;
            newAbi.LoadCardAbility(abi);
            CurrentAbilities.Add(newAbi);
        }
    }

    public override void CopyCard(Card card)
    {
        base.CopyCard(card);
        var uc = card as UnitCard;
        power = uc.StartPower;
        CurrentPower = uc.CurrentPower;
        health = uc.StartHealth;
        CurrentHealth = uc.CurrentHealth;
        MaxHealth = uc.MaxHealth;
        unitDeathSound = uc.UnitDeathSound;

        startingAbilities = uc.startingAbilities;
        CurrentAbilities = new();

        foreach (var abi in uc.CurrentAbilities)
        {
            if (abi == null)
            {
                Debug.LogError("EMPTY ABILITY!");
                continue;
            }

            // Don't copy ChangeControl abilities
            if (abi is TriggeredAbility tra)
            {
                foreach (var eg in tra.EffectGroupList)
                    foreach (var e in eg.Effects)
                        if (e is ChangeControlEffect)
                            goto NextAbility;
            }

            var newAbi = CreateInstance(abi.GetType().Name) as CardAbility;
            newAbi.LoadCardAbility(abi);
            CurrentAbilities.Add(abi);

        NextAbility:;
        }
    }
}
