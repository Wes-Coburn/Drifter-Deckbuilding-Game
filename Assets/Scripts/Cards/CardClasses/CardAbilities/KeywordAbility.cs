using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Keyword Ability", menuName = "Card Abilities/Keyword Ability")]

public class KeywordAbility : CardAbility
{
    public KeywordTrigger KeywordTrigger;
    public List<Effect> Effects;
}
