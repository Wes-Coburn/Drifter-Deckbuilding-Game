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
        string abilityName = "";
        string abilityDescription = "";
        if (AbilityScript is StaticAbility)
        {
            abilityName = AbilityScript.AbilityName;
            AbilitySprite = AbilityScript.AbilitySprite;
            abilityDescription = AbilityScript.AbilityDescription;
        }
        else if (AbilityScript is TriggeredAbility)
        {
            TriggeredAbility keywordAbility = AbilityScript as TriggeredAbility;
            AbilityTrigger keywordTrigger = keywordAbility.AbilityTrigger;

            abilityName = keywordTrigger.AbilityName;
            abilityDescription = "Does something when " + keywordTrigger.AbilityDescription;
            AbilitySprite = keywordTrigger.AbilitySprite;
        }
        AbilityDescription = abilityName + ": " + abilityDescription;
    }
}
