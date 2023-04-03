using TMPro;
using UnityEngine;

public class ActionCardDisplay : CardDisplay
{
    [SerializeField] private GameObject cardDescription;
    public ActionCard ActionCard { get => CardScript as ActionCard; }

    /******
     * *****
     * ****** DISPLAY_CARD
     * *****
     *****/
    protected override void DisplayCard()
    {
        base.DisplayCard();
        cardDescription.GetComponent<TextMeshProUGUI>()
            .SetText(Managers.CA_MAN.FilterKeywords(ActionCard.EffectDescription));
    }

    /******
     * *****
     * ****** DISPLAY_ZOOM_CARD
     * *****
     *****/
    public override void DisplayZoomCard(GameObject parentCard, bool isBaseZoomCard = false)
    {
        base.DisplayZoomCard(parentCard, isBaseZoomCard);

        var acd = parentCard.GetComponent<ActionCardDisplay>();
        string description = acd.ActionCard.EffectDescription;
        DisplayZoomCard_Finish(description);
    }
    public override void DisplayZoomCard(Card card, bool isBaseZoomCard = false)
    {
        base.DisplayZoomCard(card, isBaseZoomCard);

        var ac = card as ActionCard;
        string description = ac.EffectDescription;
        DisplayZoomCard_Finish(description);
    }
    private void DisplayZoomCard_Finish(string description)
    {
        var tmPro = cardDescription.GetComponent<TextMeshProUGUI>();
        tmPro.SetText(Managers.CA_MAN.FilterKeywords(description));
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

    public override void ResetCard()
    {
        base.ResetCard();
        ResetEffects();
    }
    public override void DisableVisuals() => base.DisableVisuals();
}
