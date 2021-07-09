using UnityEngine;
using TMPro;

public class ActionCardDisplay : CardDisplay
{
    /* HERO_CARD_SCRIPTABLE_OBJECT */
    public ActionCard ActionCard { get => CardScript as ActionCard; }
    [SerializeField] private GameObject cardDescription;

    /******
     * *****
     * ****** DISPLAY_CARD
     * *****
     *****/
    protected override void DisplayCard()
    {
        base.DisplayCard();
        cardDescription.GetComponent<TextMeshPro>().SetText(CardScript.CardDescription);
    }

    /******
     * *****
     * ****** DISPLAY_ZOOM_CARD
     * *****
     *****/
    public override void DisplayZoomCard(GameObject parentCard)
    {
        base.DisplayZoomCard(parentCard);
        ActionCardDisplay acd = parentCard.GetComponent<CardDisplay>() as ActionCardDisplay;
        cardDescription.GetComponent<TextMeshPro>().SetText(acd.CardScript.CardDescription);
    }
}
