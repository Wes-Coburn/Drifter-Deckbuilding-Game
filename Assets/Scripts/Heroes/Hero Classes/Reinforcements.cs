using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reinforcements", menuName = "Heroes/NPC/Enemy Hero/Reinforcements")]
public class Reinforcements : ScriptableObject
{
    public List<UnitCard> StartingUnits;
    public List<UnitCard> ReinforcementUnits;
    public List<ActionCard> ReinforcementActions;
}
