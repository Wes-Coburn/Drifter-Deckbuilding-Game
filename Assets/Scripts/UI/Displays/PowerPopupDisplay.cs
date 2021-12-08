using UnityEngine;
using TMPro;

public class PowerPopupDisplay : MonoBehaviour
{
    /* POWER_SCRIPTABLE_OBJECT */
    private HeroPower powerScript;
    public HeroPower PowerScript
    {
        get => powerScript;
        set
        {
            powerScript = value;
            DisplayHeroPower();
        }
    }

    public Sprite PowerSprite
    {
        set => powerSprite.GetComponent<SpriteRenderer>().sprite = value;
    }
    [SerializeField] private GameObject powerSprite;

    public string PowerDescription
    {
        set => powerDescription.GetComponent<TextMeshPro>().SetText(value);
    }
    [SerializeField] private GameObject powerDescription;

    private void DisplayHeroPower()
    {
        PowerSprite = PowerScript.PowerSprite;
        int cost = powerScript.PowerCost;
        PowerDescription = PowerScript.PowerName + 
            " (" + cost + " energy): " + PowerScript.PowerDescription;
    }
}
