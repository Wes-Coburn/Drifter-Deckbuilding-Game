using UnityEngine;
using TMPro;

public class AbilityPopupDisplay : MonoBehaviour
{
    /* ABILITY_SCRIPTABLE_OBJECT */
    private CardAbility abilityScript;
    public CardAbility AbilityScript
    {
        get => abilityScript;
        set
        {
            abilityScript = value;
            DisplayAbilityPopup();
        }
    }

    /* ABILITY_DATA */
    public Sprite AbilitySprite
    {
        set => abilitySprite.GetComponent<SpriteRenderer>().sprite = value;
    }
    [SerializeField] private GameObject abilitySprite;

    public string AbilityDescription
    {
        set => abilityDescription.GetComponent<TextMeshPro>().SetText(value);
    }
    [SerializeField] private GameObject abilityDescription;

    /******
     * *****
     * ****** DISPLAY_ABILITY_POPUP
     * *****
     *****/
    private void DisplayAbilityPopup()
    {
        string name;
        string description;
        if (AbilityScript is StaticAbility)
        {
            name = AbilityScript.AbilityName;
            description = AbilityScript.AbilityDescription;
            AbilitySprite = AbilityScript.AbilitySprite;
        }
        else if (AbilityScript is TriggeredAbility ta)
        {
            AbilityTrigger trigger = ta.AbilityTrigger;
            name = trigger.AbilityName;
            description = "Does something when " + trigger.AbilityDescription;
            AbilitySprite = trigger.AbilitySprite;
        }
        else
        {
            Debug.LogError("SCRIPT TYPE NOT FOUND!");
            return;
        }
        AbilityDescription = name + ": " + description;
    }
}
