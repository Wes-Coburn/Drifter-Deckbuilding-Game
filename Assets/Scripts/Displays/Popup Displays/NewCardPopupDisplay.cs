using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

public class NewCardPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupTitle;
    [SerializeField] private GameObject newCardZone;
    [SerializeField] private GameObject newCardChest;
    [SerializeField] private GameObject[] addCardButtons;
    [SerializeField] private GameObject ignoreCardButton;
    [SerializeField] private GameObject redrawCardsButton;

    private CardManager caMan;
    private PlayerManager pMan;
    private DialogueManager dMan;
    private UIManager uMan;
    private AnimationManager anMan;
    private GameManager gMan;

    private int redrawCost;
    private Card newCard;
    private Card[] chooseCards;

    public string PopupTitle
    {
        get
        {
            return popupTitle.GetComponent<TextMeshProUGUI>().text;
        }
        set
        {
            popupTitle.GetComponent<TextMeshProUGUI>().text = value;
        }
    }
    public Card NewCard
    {
        set
        {
            newCard = value;
            DisplayNewCardChest();
        }
    }

    public Card[] ChooseCards
    {
        set
        {
            chooseCards = value;
            DisplayNewCardChest();
        }
    }

    private void Awake()
    {
        caMan = CardManager.Instance;
        pMan = PlayerManager.Instance;
        dMan = DialogueManager.Instance;
        uMan = UIManager.Instance;
        anMan = AnimationManager.Instance;
        gMan = GameManager.Instance;

        ignoreCardButton.GetComponentInChildren<TextMeshProUGUI>().SetText
            ("Take " + GameManager.IGNORE_CARD_AETHER + " Aether");

        if (pMan.GetAugment("Quadraphonic Deliberator")) redrawCost = 1;
        else redrawCost = GameManager.REDRAW_CARDS_AETHER;

        redrawCardsButton.GetComponentInChildren<TextMeshProUGUI>().SetText
            ("Redraw (" + redrawCost + " Aether)");

        GetComponent<SoundPlayer>().PlaySound(0);
    }

    private void DisplayNewCardChest()
    {
        newCardChest.SetActive(true);
        foreach (GameObject button in addCardButtons) button.SetActive(false);
        ignoreCardButton.SetActive(false);
        redrawCardsButton.SetActive(false);
        anMan.CreateParticleSystem(newCardChest, ParticleSystemHandler.ParticlesType.NewCard, 5);
    }

    private void SwitchToCards()
    {
        GetComponent<SoundPlayer>().PlaySound(1);
        newCardChest.SetActive(false);
        foreach (GameObject button in addCardButtons) button.SetActive(true);
        ignoreCardButton.SetActive(true);
        anMan.CreateParticleSystem(null, ParticleSystemHandler.ParticlesType.ButtonPress);
    }

    public void DisplayNewCard()
    {
        SwitchToCards();
        // Card Popup
        GameObject newCard = caMan.ShowCard(this.newCard, new Vector2(), CardManager.DisplayType.NewCard);
        if (newCard == null)
        {
            Debug.LogError("CARD IS NULL!");
            return;
        }
        CardZoom cz = newCard.GetComponent<CardZoom>();
        newCard.transform.SetParent(newCardZone.transform, false);
        // Description Popup
        cz.CreateDescriptionPopup(new Vector2(-550, 0), 3);
        CardZoom.DescriptionPopup.transform.SetParent(newCardZone.transform, true);
        // Ability Popups
        cz.CreateAbilityPopups(new Vector2(550, 0), 3, false);
        CardZoom.AbilityPopupBox.transform.SetParent(newCardZone.transform, true);
        cz.enabled = false; // Disable more info tooltip
        // Card Popup
        newCard.transform.SetAsLastSibling();
        // Prevent DestroyZoomObjects() on ZoomAbilityIcon
        CardZoom.CurrentZoomCard = null;
        CardZoom.DescriptionPopup = null;
        CardZoom.AbilityPopupBox = null;
    }

    public void DisplayChooseCards()
    {
        SwitchToCards();
        redrawCardsButton.SetActive(true);

        foreach (Card card in chooseCards)
        {
            // Card Popup
            GameObject newCard = CardManager.Instance.ShowCard(card, new Vector2(),
                CardManager.DisplayType.ChooseCard); // TESTING
            CardDisplay cd = newCard.GetComponent<CardDisplay>();
            newCard.transform.SetParent(newCardZone.transform, false);
            cd.DisableVisuals();
            newCard.transform.localScale = new Vector2(3, 3);
        }

        foreach (GameObject button in addCardButtons)
        {
            button.transform.localPosition =
                new Vector2(button.transform.localPosition.x, -400);
        }
    }

    public void AddCard_OnClick(int cardSelection)
    {
        GetComponent<SoundPlayer>().PlaySound(2);
        uMan.DestroyInteractablePopup(gameObject);
        DisableButtons();

        Card newCard;
        if (cardSelection == 0) newCard = this.newCard;
        else newCard = chooseCards[cardSelection - 1];
        CardManager.Instance.AddCard(newCard, GameManager.PLAYER, true);

        if (SceneLoader.IsActiveScene(SceneLoader.Scene.HomeBaseScene))
        {
            RewardBonusAugment();
            return;
        }

        DialogueClip nextClip = dMan.EngagedHero.NextDialogueClip;
        if (!SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene))
        {
            DialoguePrompt dp = nextClip as DialoguePrompt;
            if (dp.AetherCells > 0) uMan.CreateAetherCellPopup(dp.AetherCells);
            else dMan.DisplayDialoguePopup();
        }
        else if (nextClip is CombatRewardClip crc)
        {
            int aetherReward = gMan.GetAetherReward((dMan.EngagedHero as EnemyHero).EnemyLevel);
            if (aetherReward > 0) uMan.CreateAetherCellPopup(aetherReward);
            else
            {
                dMan.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
                SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
            }
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }

    public void IgnoreCard_OnClick()
    {
        GetComponent<SoundPlayer>().PlaySound(3);
        pMan.AetherCells += GameManager.IGNORE_CARD_AETHER;
        uMan.DestroyInteractablePopup(gameObject);
        DisableButtons();

        if (SceneLoader.IsActiveScene(SceneLoader.Scene.HomeBaseScene))
        {
            RewardBonusAugment();
            return;
        }

        DialogueClip nextClip = dMan.EngagedHero.NextDialogueClip;
        if (!SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene))
        {
            DialoguePrompt dp = nextClip as DialoguePrompt;
            if (dp.AetherCells > 0) uMan.CreateAetherCellPopup(dp.AetherCells);
            else dMan.DisplayDialoguePopup();
        }
        else if (dMan.EngagedHero.NextDialogueClip is CombatRewardClip crc)
        {
            int aetherReward = gMan.GetAetherReward((dMan.EngagedHero as EnemyHero).EnemyLevel);
            if (aetherReward > 0) uMan.CreateAetherCellPopup(aetherReward);
            else
            {
                dMan.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
                SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
            }
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }

    public void RedrawCards_OnClick()
    {
        GetComponent<SoundPlayer>().PlaySound(3);

        if (pMan.AetherCells < redrawCost)
        {
            uMan.InsufficientAetherPopup();
            return;
        }

        pMan.AetherCells -= redrawCost;
        CardManager.ChooseCard chooseCardType;
        if (chooseCards[0] is UnitCard) chooseCardType = CardManager.ChooseCard.Unit;
        else chooseCardType = CardManager.ChooseCard.Action;

        uMan.CreateNewCardPopup(null, PopupTitle,
            CardManager.Instance.ChooseCards(chooseCardType));
    }

    private void RewardBonusAugment()
    {
        string aetherMagnet = "Aether Magnet";
        if (pMan.GetAugment(aetherMagnet))
        {
            anMan.TriggerAugment(aetherMagnet);
            uMan.CreateAetherCellPopup(GameManager.AETHER_MAGNET_REWARD);
        }
    }

    private void DisableButtons()
    {
        foreach (GameObject button in addCardButtons)
            button.GetComponent<Button>().interactable = false;
        ignoreCardButton.GetComponent<Button>().interactable |= false;
    }
}
