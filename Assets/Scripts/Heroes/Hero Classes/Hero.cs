using UnityEngine;

public abstract class Hero : ScriptableObject
{
    [Header("HERO NAME")]
    public string HeroName;
    [Header("HERO PORTRAIT")]
    public Sprite HeroPortrait;
    [Header("HERO DESCRIPTION")]
    [TextArea]
    public string HeroDescription;
    [Header("HERO SOUNDS")]
    public Sound HeroWin;
    public Sound HeroLose;

    public virtual void LoadHero(Hero hero)
    {
        HeroName = hero.HeroName;
        HeroPortrait = hero.HeroPortrait;
        HeroDescription = hero.HeroDescription;
        HeroWin = hero.HeroWin;
        HeroLose = hero.HeroLose;
    }
}
