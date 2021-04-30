using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * 
 * Do we need this class? Heroes have the scriptable objects on them already, and if they gain 
 * or lose an ability somehow, then that script could be on the card that gives it to them...
 * 
 * 
 */

public class CardAbilityLibrary : MonoBehaviour
{
    /* CARD_ABILITY_LIST */
    private List<CardAbility> cardAbilityScripts = new List<CardAbility>();
    /* CARD_ABILITY_SCRIPTS */
    public CardAbility Evasion;
    public CardAbility Retaliate;
    public CardAbility Stealth;
    public CardAbility Ward;
    
    public void Awake()
    {    
        cardAbilityScripts.Add(Evasion);
        cardAbilityScripts.Add(Retaliate);
        cardAbilityScripts.Add(Stealth);
        cardAbilityScripts.Add(Ward);
    }

    public ScriptableObject GetAbilityScript(string abilityName)
    {
        foreach (CardAbility cardAbilityScript in cardAbilityScripts)
        {
            if (cardAbilityScript.AbilityName == abilityName) return cardAbilityScript;
        }
        Debug.Log("ABILITY " + abilityName + " NOT FOUND!!!");
        return null;
    }
}
