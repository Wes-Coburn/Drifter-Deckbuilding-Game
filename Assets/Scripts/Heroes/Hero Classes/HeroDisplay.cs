using UnityEngine;

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

    public Sprite HeroPortrait
    {
        set => heroPortrait.GetComponent<SpriteRenderer>().sprite = value;
    }
    [SerializeField] private GameObject heroPortrait;

    public virtual void DisplayHero()
    {
        //HeroPortrait = heroScript.HeroPortrait;
    }
}
