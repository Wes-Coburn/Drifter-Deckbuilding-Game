using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Hero", menuName = "Heroes/NPC/Enemy/Enemy Hero")]
public class EnemyHero : NPCHero
{
    [Header("ENEMY REINFORCEMNTS")]
    [SerializeField] private List<Reinforcements> reinforcements;
    public List<Reinforcements> Reinforcements { get => reinforcements; }

    public override void LoadHero(Hero hero)
    {
        base.LoadHero(hero);
        EnemyHero eh = hero as EnemyHero;
        reinforcements = eh.Reinforcements;
    }
}
