using UnityEngine;
using UnityEngine.UI;
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
            heroPortrait.GetComponent<Image>().sprite = value;
            
            if (this is PlayerHeroDisplay)
            {
                UIManager.Instance.GetPortraitPosition
                    (HeroScript.HeroName, out Vector2 position, out Vector2 scale, SceneLoader.Scene.CombatScene);
                heroPortrait.transform.localPosition = position;
                heroPortrait.transform.localScale = scale;
            }
        }
    }
    [SerializeField] private GameObject heroPortrait;

    public int HeroHealth
    {
        set => heroHealth.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
    }
    [SerializeField] private GameObject heroHealth;

    public virtual void DisplayHero()
    {
        HeroPortrait = heroScript.HeroPortrait;
    }
}
