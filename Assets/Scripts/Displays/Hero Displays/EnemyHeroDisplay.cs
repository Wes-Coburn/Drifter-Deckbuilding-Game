using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHeroDisplay : HeroDisplay
{
    [SerializeField] private GameObject heroPower;
    [SerializeField] private GameObject powerImage;

    public EnemyHero EnemyHero { get => HeroScript as EnemyHero; }
    public GameObject HeroPower { get => heroPower; }
    public GameObject PowerImage { get => powerImage; }

    public override void DisplayHero()
    {
        base.DisplayHero();
        EnemyHero eh = HeroScript as EnemyHero;
        if (eh.EnemyHeroPower == null) heroPower.SetActive(false);
        else powerImage.GetComponent<Image>().sprite = eh.EnemyHeroPower.PowerSprite;
    }
}
