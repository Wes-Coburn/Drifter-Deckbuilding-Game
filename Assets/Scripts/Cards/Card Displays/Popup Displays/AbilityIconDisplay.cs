using UnityEngine;
using TMPro;

public class AbilityIconDisplay : MonoBehaviour
{
    /* ABILITY_SCRIPTABLE_OBJECT */
    private CardAbility abilityScript;
    public CardAbility AbilityScript
    {
        get => abilityScript;
        set
        {
            abilityScript = value;
            DisplayAbilityIcon();
        }
    }
    public CardAbility ZoomAbilityScript
    {
        get => abilityScript;
        set
        {
            abilityScript = value;
            DisplayZoomAbilityIcon();
        }
    }

    /* ABILITY_DATA */
    public GameObject AbilitySprite;
    public GameObject AbilityName;

    /******
     * *****
     * ****** DISPLAY_ABILITY_ICON
     * *****
     *****/
    private void DisplayAbilityIcon()
    {
        Sprite abilitySprite;
        if (AbilityScript is StaticAbility) 
            abilitySprite = AbilityScript.AbilitySprite;
        else if (AbilityScript is TriggeredAbility ta)
        {
            AbilityTrigger trigger = ta.AbilityTrigger;
            abilitySprite = trigger.AbilitySprite;
        }
        else
        {
            Debug.LogError("SCRIPT TYPE NOT FOUND!");
            return;
        }
        SetAbilityIcon(abilitySprite);
    }

    /******
     * *****
     * ****** DISPLAY_ZOOM_ABILITY_ICON
     * *****
     *****/
    private void DisplayZoomAbilityIcon()
    {
        DisplayAbilityIcon();
        string abilityName;
        if (AbilityScript is StaticAbility) 
            abilityName = AbilityScript.AbilityName;
        else if (AbilityScript is TriggeredAbility keywordAbility) 
            abilityName = keywordAbility.AbilityDescription;
        else
        {
            Debug.LogError("SCRIPT TYPE NOT FOUND!");
            return;
        }
        SetAbilityName(abilityName);
    }

    /******
     * *****
     * ****** SETTERS
     * *****
     *****/
    private void SetAbilityIcon(Sprite abilitySprite) => AbilitySprite.GetComponent<SpriteRenderer>().sprite = abilitySprite;
    private void SetAbilityName(string abilityDescription)
    {
        TextMeshPro txtPro = AbilityName.GetComponent<TextMeshPro>();
        txtPro.SetText(abilityDescription);
    }
}
