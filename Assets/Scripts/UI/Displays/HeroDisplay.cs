using UnityEngine;
using TMPro;

public abstract class HeroDisplay : MonoBehaviour
{
    /* HERO_SCRIPTABLE_OBJECT */
    private Hero heroScript;
    public Hero HeroScript
    {
        get => heroScript;
        set
        {
            heroScript = value;
            DisplayHero();
        }
    }

    public GameObject HeroFrame { get => heroFrame; }
    public GameObject HeroStats { get => heroStats; }
    [SerializeField] private GameObject heroFrame;
    [SerializeField] private GameObject heroStats;

    public Sprite HeroPortrait
    {
        set
        {
            heroPortrait.GetComponent<SpriteRenderer>().sprite = value;
            if (this is PlayerHeroDisplay)
            {
                UIManager.PositionAndScale pas = UIManager.Instance.GetPortraitPosition
                    (HeroScript.HeroName, SceneLoader.Scene.CombatScene);
                heroPortrait.transform.localPosition = pas.Position;
                heroPortrait.transform.localScale = pas.Scale;
            }
        }
    }
    [SerializeField] private GameObject heroPortrait;

    public int HeroHealth
    {
        set => heroHealth.GetComponent<TextMeshPro>().SetText(value.ToString());
    }
    [SerializeField] private GameObject heroHealth;

    public virtual void DisplayHero()
    {
        HeroPortrait = heroScript.HeroPortrait;
    }
}
