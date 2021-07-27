using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reinforcements", menuName = "Heroes/Reinforcements")]
public class Reinforcements : ScriptableObject
{
    public List<int> ReinforcementSchedule;
    public List<UnitCard> ReinforcementUnits;
}
