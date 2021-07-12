using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Follower Card", menuName = "Cards/Follower")]
public class FollowerCard : Card
{
    public int CurrentPower { get; set; }
    public int CurrentDefense { get; set; }
    public int MaxDefense { get; set; }
    public bool IsExhausted { get; set; }

    public string LevelUpCondition { get => levelUpCondition; }
    [SerializeField] private string levelUpCondition;

    public int StartPower { get => power; }
    [SerializeField] private int power;

    public int StartDefense { get => defense; }
    [SerializeField] private int defense;

    public List<CardAbility> Level1Abilities { get => level1Abilities; }
    [SerializeField] private List<CardAbility> level1Abilities;

    public List<CardAbility> Level2Abilities { get => level2Abiliites; }
    [SerializeField] private List<CardAbility> level2Abiliites;

    public override void LoadCard(Card card)
    {
        base.LoadCard(card);
        FollowerCard fc = card as FollowerCard;
        levelUpCondition = fc.LevelUpCondition;
        power = fc.StartPower;
        defense = fc.StartDefense;
        level1Abilities = fc.Level1Abilities;
        level2Abiliites = fc.Level2Abilities;
    }
}
