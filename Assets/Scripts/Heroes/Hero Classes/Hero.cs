using UnityEngine;

public abstract class Hero : ScriptableObject
{
    [TextArea]
    [SerializeField] private string developerNotes;
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

    private string DeveloperNotes { get => developerNotes; }
    public string HeroName { get => heroName; }
    public string HeroShortName { get => heroShortName; }
    public Sprite HeroPortrait { get => heroPortrait; }
    public string HeroDescription { get => heroDescription; }
    public Sound HeroWin { get => heroWin; }
    public Sound HeroLose { get => heroLose; }

    public virtual void LoadHero(Hero hero)
    {
        developerNotes = hero.DeveloperNotes;
        heroName = hero.HeroName;
        heroShortName = hero.heroShortName;
        heroPortrait = hero.HeroPortrait;
        heroDescription = hero.HeroDescription;
        heroWin = hero.HeroWin;
        heroLose = hero.HeroLose;
    }
}
