using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CardAbility : ScriptableObject
{
    public string AbilityName;
    public string AbilityDescription;
    public Sprite AbilitySprite;
    public bool OverrideColor;
    public Color AbilityColor;
    public List<CardAbility> LinkedAbilites;

    public virtual void LoadCardAbility(CardAbility cardAbility)
    {
        name = cardAbility.name; // For LoadAbilities in GameLoader

        AbilityName = cardAbility.AbilityName;
        AbilityDescription = cardAbility.AbilityDescription;
        AbilitySprite = cardAbility.AbilitySprite;
        LinkedAbilites = new();
        OverrideColor = cardAbility.OverrideColor;
        AbilityColor = cardAbility.AbilityColor;
        LinkedAbilites = cardAbility.LinkedAbilites.ToList();
    }
}
