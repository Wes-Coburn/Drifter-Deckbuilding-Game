using System.Collections.Generic;
using UnityEngine;

public abstract class CardAbility : ScriptableObject
{
    public string AbilityName;
    public string AbilityDescription;
    public Sprite AbilitySprite;
    public List<CardAbility> LinkedAbilites;
}
