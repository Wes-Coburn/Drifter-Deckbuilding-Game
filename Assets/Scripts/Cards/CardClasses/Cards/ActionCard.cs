using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action Card", menuName = "Cards/Action")]
public class ActionCard : Card
{
    [Header("ACTION CARD EFFECTS")]
    public List<Effect> Effects;
}
