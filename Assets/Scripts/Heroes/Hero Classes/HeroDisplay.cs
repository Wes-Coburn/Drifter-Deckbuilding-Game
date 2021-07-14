using UnityEngine;

public class HeroDisplay : MonoBehaviour
{
    /* POWER_SCRIPTABLE_OBJECT */
    private Hero heroScript;
    public Hero HeroScript
    {
        get => heroScript;
        set
        {
            heroScript = value;
            DisplayPower();
        }
    }
    public Sprite PowerImage
    {
        set => powerImage.GetComponent<SpriteRenderer>().sprite = value;
    }
    [SerializeField] private GameObject powerImage;

    private void DisplayPower()
    {
        PowerImage = HeroScript.HeroPower.PowerSprite;
    }
}
