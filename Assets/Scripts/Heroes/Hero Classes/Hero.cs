using UnityEngine;

public abstract class Hero : ScriptableObject
{
    [Header("DEVELOPER NOTES")]
    [TextArea]
    [SerializeField]
    private string developerNotes;
    [Header("HERO NAME")]
    private string heroName;
    [Header("HERO PORTRAIT")]
    private Sprite heroPortrait;
    [Header("HERO DESCRIPTION")]
    [TextArea]
    private string heroDescription;
    [Header("HERO SOUNDS")]
    private Sound heroWin;
    private Sound heroLose;

    private string DeveloperNotes { get => developerNotes; }
    public string HeroName { get => heroName; }
    public Sprite HeroPortrait { get => heroPortrait; }
    public string HeroDescription { get => heroDescription; }
    public Sound HeroWin { get => heroWin; }
    public Sound HeroLose { get => heroLose; }

    public virtual void LoadHero(Hero hero)
    {
        developerNotes = hero.DeveloperNotes;
        heroName = hero.HeroName;
        heroPortrait = hero.HeroPortrait;
        heroDescription = hero.HeroDescription;
        heroWin = hero.HeroWin;
        heroLose = hero.HeroLose;
    }
}
