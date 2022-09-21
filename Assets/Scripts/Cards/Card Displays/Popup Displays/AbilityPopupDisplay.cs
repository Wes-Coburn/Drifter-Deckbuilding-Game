using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject abilitySprite;
    [SerializeField] private GameObject abilityDescription;

    private CardAbility abilityScript;
    private CardManager caMan;

    public Sprite AbilitySprite
    {
        set => abilitySprite.GetComponent<Image>().sprite = value;
    }
    public string AbilityDescription
    {
        set => abilityDescription.GetComponent<TextMeshProUGUI>().SetText(value);
    }

    private void Awake()
    {
        caMan = CardManager.Instance;
    }

    /******
     * *****
     * ****** DISPLAY_ABILITY_POPUP
     * *****
     *****/
    public void DisplayAbilityPopup(CardAbility cardAbility, bool isAbilityZoom, bool isPlayerSource)
    {
        abilityScript = cardAbility; // TESTING

        string name;
        string description;
        
        if (abilityScript is StaticAbility)
        {
            AbilitySprite = abilityScript.AbilitySprite;
            if (!isAbilityZoom)
            {
                name = abilityScript.AbilityName;
                description = abilityScript.AbilityDescription;
            }
            else
            {
                name = "";
                description = abilityScript.AbilityDescription;
            }
        }
        else if (abilityScript is TriggeredAbility ta)
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
            if (abilityScript == null) Debug.LogError("SCRIPT IS NULL!");
            else Debug.LogError("SCRIPT TYPE NOT FOUND!");
            return;
        }
        if (!string.IsNullOrEmpty(name)) name += ": ";

        string filteredDescription = caMan.FilterKeywords(name + description);
        filteredDescription = caMan.FilterCreatedCardProgress(filteredDescription, isPlayerSource); // TESTING
        AbilityDescription = CardManager.Instance.FilterKeywords(filteredDescription);
        abilitySprite.GetComponent<Image>().color = caMan.GetAbilityColor(abilityScript); // TESTING
    }
}
