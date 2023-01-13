using UnityEngine;

public abstract class Hero : ScriptableObject
{
    [Header("HERO NAME")]
    [SerializeField] private string heroName;
    [SerializeField] private string heroShortName;
    [Header("HERO PORTRAIT")]
    [SerializeField] private Sprite heroPortrait;
    [Header("HERO DESCRIPTION")]
    [TextArea]
    [SerializeField] private string heroDescription;
    [Header("HERO SOUNDS")]
    [SerializeField] private Sound heroWin;
    [SerializeField] private Sound heroLose;
    [Header("HERO POWER")]
    [SerializeField] private HeroPower heroPower;

    public string HeroName { get => heroName; }
    public string HeroShortName { get => heroShortName; }
    public Sprite HeroPortrait { get => heroPortrait; }
    public string HeroDescription { get => heroDescription; }
    public Sound HeroWin { get => heroWin; }
    public Sound HeroLose { get => heroLose; }
    public HeroPower HeroPower { get => heroPower; }

    public virtual void LoadHero(Hero hero)
    {
        heroName = hero.HeroName;
        heroShortName = hero.heroShortName;
        heroPortrait = hero.HeroPortrait;
        heroDescription = hero.HeroDescription;
        heroWin = hero.HeroWin;
        heroLose = hero.HeroLose;
        heroPower = hero.HeroPower;
    }
}
