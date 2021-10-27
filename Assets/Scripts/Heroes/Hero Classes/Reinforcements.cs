using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reinforcements", menuName = "Heroes/NPC/Enemy/Reinforcements")]
public class Reinforcements : ScriptableObject
{
    public ReinforcementSchedule ReinforcementSchedule;
    public List<UnitCard> ReinforcementUnits;
}
