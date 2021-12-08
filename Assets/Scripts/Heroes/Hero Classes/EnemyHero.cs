using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Hero", menuName = "Heroes/NPC/Enemy Hero/Enemy Hero")]
public class EnemyHero : NPCHero
{
    [Header("ENEMY REINFORCEMNTS")]
    [SerializeField] private List<Reinforcements> reinforcements;
    [Header("IS BOSS")]
    [SerializeField] private bool isBoss;
    public List<Reinforcements> Reinforcements { get => reinforcements; }
    public bool IsBoss { get => isBoss; }

    public override void LoadHero(Hero hero)
    {
        base.LoadHero(hero);
        EnemyHero eh = hero as EnemyHero;
        reinforcements = eh.Reinforcements;
        isBoss = eh.IsBoss;
    }
}
