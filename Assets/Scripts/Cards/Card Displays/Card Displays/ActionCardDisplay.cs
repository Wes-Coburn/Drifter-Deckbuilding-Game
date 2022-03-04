using UnityEngine;
using TMPro;

public class ActionCardDisplay : CardDisplay
{
    private CardManager caMan;
    [SerializeField] private GameObject cardDescription;
    public ActionCard ActionCard { get => CardScript as ActionCard; }

    private void Awake() => caMan = CardManager.Instance;
    /******
     * *****
     * ****** DISPLAY_CARD
     * *****
     *****/
    protected override void DisplayCard()
    {
        base.DisplayCard();
        cardDescription.GetComponent<TextMeshProUGUI>().SetText(caMan.FilterKeywords(ActionCard.EffectDescription));
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
        tmPro.SetText(caMan.FilterKeywords(description));
    }

    /******
     * *****
     * ****** DISPLAY_CARD_PAGE_CARD
     * *****
     *****/
    public override void DisplayCardPageCard(Card card)
    {
        base.DisplayCardPageCard(card);
        CardScript = card;
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
