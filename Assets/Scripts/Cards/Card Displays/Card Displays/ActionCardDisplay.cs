using UnityEngine;
using TMPro;

public class ActionCardDisplay : CardDisplay
{
    /* ACTION_CARD_SCRIPTABLE_OBJECT */
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
        cardDescription.GetComponent<TextMeshPro>().SetText(ActionCard.EffectDescription);
    }

    /******
     * *****
     * ****** DISPLAY_ZOOM_CARD
     * *****
     *****/
    public override void DisplayZoomCard(GameObject parentCard, Card card = null)
    {
        base.DisplayZoomCard(parentCard, card);

        if (card == null)
        {
            ActionCardDisplay acd = parentCard.GetComponent<CardDisplay>() as ActionCardDisplay;
            cardDescription.GetComponent<TextMeshPro>().SetText(acd.ActionCard.EffectDescription);
        }
        else
        {
            ActionCard ac = card as ActionCard;
            cardDescription.GetComponent<TextMeshPro>().SetText(ac.EffectDescription);
        }
    }
}
