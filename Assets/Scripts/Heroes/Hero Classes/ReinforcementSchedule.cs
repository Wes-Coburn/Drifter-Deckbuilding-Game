using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reinforcement Schedule", menuName = "Heroes/NPC/Enemy/Reinforcement Schedule")]
public class ReinforcementSchedule : ScriptableObject
{
    public List<int> Schedule;
}
