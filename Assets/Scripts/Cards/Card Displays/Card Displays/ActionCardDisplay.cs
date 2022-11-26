using UnityEngine;
using TMPro;

public class ActionCardDisplay : CardDisplay
{
    private CardManager caMan;
    [SerializeField] private GameObject cardDescription;
    public ActionCard ActionCard { get => CardScript as ActionCard; }

    protected void Awake()
    {
        //base.Awake();
        caMan = CardManager.Instance;
    }
    
    protected override void DisplayCard()
    {
        base.DisplayCard();
        cardDescription.GetComponent<TextMeshProUGUI>().SetText(caMan.FilterKeywords(ActionCard.EffectDescription));
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
        tmPro.SetText(caMan.FilterKeywords(description));
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
