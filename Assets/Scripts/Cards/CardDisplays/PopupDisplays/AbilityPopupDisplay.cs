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
    public GameObject AbilitySprite;
    public GameObject AbilityDescription;

    /******
     * *****
     * ****** DISPLAY_ABILITY_POPUP
     * *****
     *****/
    private void DisplayAbilityPopup()
    {
        string abilityName = "";
        string abilityDescription = "";
        Sprite abilitySprite = null;
        if (AbilityScript is StaticAbility)
        {
            abilityName = AbilityScript.AbilityName;
            abilitySprite = AbilityScript.AbilitySprite;
            abilityDescription = AbilityScript.AbilityDescription;
        }
        else if (AbilityScript is KeywordAbility)
        {
            KeywordAbility keywordAbility = AbilityScript as KeywordAbility;
            KeywordTrigger keywordTrigger = keywordAbility.KeywordTrigger;

            abilityName = keywordTrigger.AbilityName;
            abilityDescription = "Does something when " + keywordTrigger.AbilityDescription;
            abilitySprite = keywordTrigger.AbilitySprite;
        }
        SetAbilityDescription(abilityName + ": " + abilityDescription);
        SetAbilitySprite(abilitySprite);
    }

    /******
     * *****
     * ****** SETTERS
     * *****
     *****/
    private void SetAbilitySprite(Sprite abilityIcon) => AbilitySprite.GetComponent<SpriteRenderer>().sprite = abilityIcon;
    private void SetAbilityDescription(string abilityDescription)
    {
        TextMeshPro txtPro = AbilityDescription.GetComponent<TextMeshPro>();
        txtPro.SetText(abilityDescription);
    }
}
