using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHeroDisplay : HeroDisplay
{
    [SerializeField] private GameObject nextReinforcements;

    [SerializeField] private GameObject heroPower;
    [SerializeField] private GameObject powerImage;

    public EnemyHero EnemyHero { get => HeroScript as EnemyHero; }
    public GameObject Reinforcements { get => nextReinforcements; }
    public GameObject HeroPower { get => heroPower; }
    public GameObject PowerImage { get => powerImage; }
    public int NextReinforcements
    {
        set => nextReinforcements.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
    }

    public override void DisplayHero()
    {
        base.DisplayHero();
        EnemyManager em = EnemyManager.Instance;
        NextReinforcements = em.ReinforcementSchedule[em.CurrentReinforcements];
        EnemyHero eh = HeroScript as EnemyHero;
        if (eh.EnemyHeroPower == null) heroPower.SetActive(false); // TESTING
        else powerImage.GetComponent<Image>().sprite = eh.EnemyHeroPower.PowerSprite;
    }
}
