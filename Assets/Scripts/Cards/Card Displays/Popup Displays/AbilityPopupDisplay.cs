using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject abilitySprite;
    [SerializeField] private GameObject abilityDescription;

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
    public Sprite AbilitySprite
    {
        set => abilitySprite.GetComponent<Image>().sprite = value;
    }
    public string AbilityDescription
    {
        set => abilityDescription.GetComponent<TextMeshProUGUI>().SetText(value);
    }

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
            if (!isAbilityZoom)
            {
                name = AbilityScript.AbilityName;
                description = AbilityScript.AbilityDescription;
            }
            else
            {
                name = "";
                description = AbilityScript.AbilityDescription;
            }
        }
        else if (AbilityScript is TriggeredAbility ta)
        {
            AbilityTrigger trigger = ta.AbilityTrigger;
            AbilitySprite = trigger.AbilitySprite;
            
            if (!isAbilityZoom)
            {
                name = trigger.AbilityName;
                description = "Does something " + trigger.AbilityDescription;
            }
            else
            {
                name = "";
                description = ta.AbilityName;
            }
        }
        else if (abilityScript is AbilityTrigger at)
        {
            AbilitySprite = at.AbilitySprite;

            if (!isAbilityZoom)
            {
                name = at.AbilityName;
                description = "Does something " + at.AbilityDescription;
            }
            else
            {
                name = "";
                description = at.AbilityDescription;
            }
        }
        else if (abilityScript is ModifierAbility ma) // TESTING
        {
            AbilitySprite = ma.AbilitySprite;

            name = "";
            description = ma.AbilityName;
        }
        else
        {
            if (AbilityScript == null) Debug.LogError("SCRIPT IS NULL!");
            else Debug.LogError("SCRIPT TYPE NOT FOUND!");
            return;
        }
        if (!string.IsNullOrEmpty(name)) name += ": ";
        AbilityDescription = CardManager.Instance.FilterKeywords(name + description);
    }
}
