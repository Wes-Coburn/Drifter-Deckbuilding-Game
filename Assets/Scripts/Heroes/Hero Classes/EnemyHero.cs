using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Hero", menuName = "Heroes/Enemy Hero")]
public class EnemyHero : NPCHero
{
    [Header("ENEMY REINFORCEMNTS")]
    public List<Reinforcements> Reinforcements;

    public override void LoadHero(Hero hero)
    {
        base.LoadHero(hero);
        EnemyHero eh = hero as EnemyHero;
        Reinforcements = eh.Reinforcements;
    }
}
