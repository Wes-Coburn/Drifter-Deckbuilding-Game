using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Hero", menuName = "Heroes/NPC/Enemy Hero/Enemy Hero")]
public class EnemyHero : NPCHero
{   
    [Header("IS BOSS")]
    [SerializeField] private bool isBoss;
    [Header("ENEMY REINFORCEMNTS")]
    [SerializeField] private List<Reinforcements> reinforcements;
    [Header("ENEMY HERO POWER")]
    [SerializeField] private EnemyHeroPower enemyHeroPower;

    public bool IsBoss { get => isBoss; }
    public List<Reinforcements> Reinforcements { get => reinforcements; }
    public EnemyHeroPower EnemyHeroPower { get => enemyHeroPower; }

    public override void LoadHero(Hero hero)
    {
        base.LoadHero(hero);
        EnemyHero eh = hero as EnemyHero;
        isBoss = eh.IsBoss;
        reinforcements = eh.Reinforcements;
        enemyHeroPower = eh.EnemyHeroPower;
    }
}
