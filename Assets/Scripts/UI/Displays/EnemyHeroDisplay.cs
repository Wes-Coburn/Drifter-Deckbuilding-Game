using UnityEngine;
using TMPro;

public class EnemyHeroDisplay : HeroDisplay
{
    [SerializeField] private GameObject nextReinforcements;
    
    public int NextReinforcements
    {
        set => nextReinforcements.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
    }

    public override void DisplayHero()
    {
        base.DisplayHero();
        EnemyManager em = EnemyManager.Instance;
        NextReinforcements = em.ReinforcementSchedule[em.CurrentReinforcements];
    }
}
