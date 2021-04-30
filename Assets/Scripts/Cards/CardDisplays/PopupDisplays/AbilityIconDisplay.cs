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
        string abilityName = "";
        Sprite abilitySprite = null;
        if (AbilityScript is StaticAbility)
        {
            abilityName = AbilityScript.AbilityName;
            abilitySprite = AbilityScript.AbilitySprite;
        }
        else if (AbilityScript is KeywordAbility)
        {
            KeywordAbility keywordAbility = (KeywordAbility)AbilityScript;
            KeywordTrigger keywordTrigger = keywordAbility.KeywordTrigger;
            abilityName = keywordTrigger.AbilityName + ": " + keywordAbility.AbilityDescription;
            abilitySprite = keywordTrigger.AbilitySprite;
        }
        SetAbilityName(abilityName);
        SetAbilityIcon(abilitySprite);
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
