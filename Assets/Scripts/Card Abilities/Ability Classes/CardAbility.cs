using System.Collections.Generic;
using UnityEngine;

public abstract class CardAbility : ScriptableObject
{
    public string AbilityName;
    public string AbilityDescription;
    public Sprite AbilitySprite;
    public List<CardAbility> LinkedAbilites;

    public virtual void LoadCardAbility(CardAbility cardAbility)
    {
        AbilityName = cardAbility.AbilityName;
        AbilityDescription = cardAbility.AbilityDescription;
        AbilitySprite = cardAbility.AbilitySprite;
        LinkedAbilites = new List<CardAbility>();
        foreach (CardAbility ca in cardAbility.LinkedAbilites)
            LinkedAbilites.Add(ca);
    }
}
