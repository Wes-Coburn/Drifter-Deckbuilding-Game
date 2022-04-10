using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Hero Power", menuName = "Heroes/NPC/Enemy Hero/Enemy Hero Power")]
public class EnemyHeroPower : HeroPower
{
    [Header("POWER TRIGGER")]
    public AbilityTrigger PowerTrigger;
}
