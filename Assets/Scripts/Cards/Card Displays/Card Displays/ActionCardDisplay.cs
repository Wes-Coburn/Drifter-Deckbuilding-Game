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
        cardDescription.GetComponent<TextMeshProUGUI>().SetText(ActionCard.EffectDescription);
    }

    /******
     * *****
     * ****** DISPLAY_ZOOM_CARD
     * *****
     *****/
    public override void DisplayZoomCard(GameObject parentCard, Card card = null)
    {
        base.DisplayZoomCard(parentCard, card);
        TextMeshProUGUI tmPro = cardDescription.GetComponent<TextMeshProUGUI>();
        string description;
        if (card == null)
        {
            ActionCardDisplay acd = parentCard.GetComponent<ActionCardDisplay>();
            description = acd.ActionCard.EffectDescription;
        }
        else
        {
            ActionCard ac = card as ActionCard;
            description = ac.EffectDescription;
        }
        tmPro.SetText(description);
    }

    /******
     * *****
     * ****** DISABLE_VISUALS
     * *****
     *****/
    public override void DisableVisuals()
    {
        base.DisableVisuals();
        // blank
    }
}
