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
    public CardAbility ZoomAbilityScript
    {
        set
        {
            abilityScript = value;
            DisplayAbilityPopup(true);
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
    private void DisplayAbilityPopup(bool isAbilityZoom = false)
    {
        string name;
        string description;
        
        if (AbilityScript is StaticAbility)
        {
            AbilitySprite = AbilityScript.AbilitySprite;
            if (isAbilityZoom)
            {
                AbilityDescription = AbilityScript.AbilityName;
                return;
            }
            else
            {
                name = AbilityScript.AbilityName;
                description = AbilityScript.AbilityDescription;
            }
        }
        else if (AbilityScript is TriggeredAbility ta)
        {
            AbilityTrigger trigger = ta.AbilityTrigger;
            AbilitySprite = trigger.AbilitySprite;
            
            if (isAbilityZoom)
            {
                AbilityDescription = AbilityScript.AbilityName;
                return;
            }
            else
            {
                name = trigger.AbilityName;
                description = "Does something when " + trigger.AbilityDescription;
            }
        }
        else if (abilityScript is AbilityTrigger at)
        {
            AbilitySprite = at.AbilitySprite;

            if (isAbilityZoom)
            {
                AbilityDescription = AbilityScript.AbilityName;
                return;
            }
            else
            {
                name = at.AbilityName;
                description = "Does something when " + at.AbilityDescription;
            }
        }
        else
        {
            if (AbilityScript == null) Debug.LogError("SCRIPT IS NULL!");
            else Debug.LogError("SCRIPT TYPE NOT FOUND!");
            return;
        }
        AbilityDescription = name + ": " + description;
    }
}
