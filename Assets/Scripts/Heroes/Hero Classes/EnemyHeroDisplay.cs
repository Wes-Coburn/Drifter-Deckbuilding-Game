using UnityEngine;
using TMPro;

public class EnemyHeroDisplay : HeroDisplay
{
    public int NextReinforcements
    {
        set => nextReinforcements.GetComponent<TextMeshPro>().SetText(value.ToString());
    }
    [SerializeField] private GameObject nextReinforcements;

    public override void DisplayHero()
    {
        base.DisplayHero();
        EnemyManager em = EnemyManager.Instance;
        NextReinforcements = em.ReinforcementSchedule[em.CurrentReinforcements];
    }
}
