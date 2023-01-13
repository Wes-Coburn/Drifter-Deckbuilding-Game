using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Hero", menuName = "Heroes/NPC/Enemy Hero/Enemy Hero")]
public class EnemyHero : NPCHero
{
    [Header("ENEMY LEVEL"), SerializeField]
    private GameManager.DifficultyLevel enemyLevel;
    [Header("ENEMY REINFORCEMNTS"), SerializeField]
    private List<Reinforcements> reinforcements;

    public GameManager.DifficultyLevel EnemyLevel { get => enemyLevel; }
    public List<Reinforcements> Reinforcements { get => reinforcements; }

    public bool IsBoss
    {
        get
        {
            int levelToInt = (int)enemyLevel;
            if (levelToInt > 1 && levelToInt % 2 != 0) return true;
            else return false;
        }
    }
    public override void LoadHero(Hero hero)
    {
        base.LoadHero(hero);
        EnemyHero eh = hero as EnemyHero;
        enemyLevel = eh.EnemyLevel;
        reinforcements = eh.Reinforcements;
    }
}
