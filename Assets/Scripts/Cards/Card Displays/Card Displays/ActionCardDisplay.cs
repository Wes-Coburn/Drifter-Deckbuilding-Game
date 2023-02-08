using TMPro;
using UnityEngine;

public class ActionCardDisplay : CardDisplay
{
    [SerializeField] private GameObject cardDescription;
    public ActionCard ActionCard { get => CardScript as ActionCard; }

    protected override void DisplayCard()
    {
        base.DisplayCard();
        cardDescription.GetComponent<TextMeshProUGUI>().SetText(ManagerHandler.CA_MAN.FilterKeywords(ActionCard.EffectDescription));
    }

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
        tmPro.SetText(ManagerHandler.CA_MAN.FilterKeywords(description));
    }

    public override void DisplayCardPageCard(Card card)
    {
        base.DisplayCardPageCard(card);
        CardScript = card;
    }

    public override void DisplayChooseCard(Card card)
    {
        base.DisplayChooseCard(card);
        DisplayCardPageCard(card);
    }

    public override void ResetCard()
    {
        base.ResetCard();
        ResetEffects();
    }
    public override void DisableVisuals() => base.DisableVisuals();
}
