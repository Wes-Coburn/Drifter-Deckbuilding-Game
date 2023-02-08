
/* Unmerged change from project 'Assembly-CSharp.Player'
Before:
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
After:
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.InteropServices;
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewCardPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupTitle;
    [SerializeField] private GameObject newCardZone;
    [SerializeField] private GameObject newCardChest;
    [SerializeField] private GameObject[] addCardButtons;
    [SerializeField] private GameObject ignoreCardButton;
    [SerializeField] private GameObject redrawCardsButton;

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
        ignoreCardButton.GetComponentInChildren<TextMeshProUGUI>().SetText
            ("Take " + GameManager.IGNORE_CARD_AETHER + " Aether");

        if (Managers.P_MAN.GetAugment("Quadraphonic Deliberator")) redrawCost = 1;
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
        Managers.AN_MAN.CreateParticleSystem(newCardChest, ParticleSystemHandler.ParticlesType.NewCard, 5);
    }

    private void SwitchToCards()
    {
        GetComponent<SoundPlayer>().PlaySound(1);
        newCardChest.SetActive(false);
        foreach (GameObject button in addCardButtons) button.SetActive(true);
        ignoreCardButton.SetActive(true);
        Managers.AN_MAN.CreateParticleSystem(null, ParticleSystemHandler.ParticlesType.ButtonPress);
    }

    public void DisplayNewCard()
    {
        SwitchToCards();
        // Card Popup
        GameObject newCard = Managers.CA_MAN.ShowCard(this.newCard, new Vector2(), CardManager.DisplayType.NewCard);
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
        CardZoom.NullifyProperties();
    }

    public void DisplayChooseCards()
    {
        SwitchToCards();
        redrawCardsButton.SetActive(true);

        foreach (Card card in chooseCards)
        {
            // Card Popup
            GameObject newCard = Managers.CA_MAN.ShowCard(card, new Vector2(),
                CardManager.DisplayType.ChooseCard);
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
        Managers.U_MAN.DestroyInteractablePopup(gameObject);
        DisableButtons();

        Card newCard = cardSelection == 0 ? this.newCard : chooseCards[cardSelection - 1];
        Managers.CA_MAN.AddCard(newCard, GameManager.PLAYER, true);

        if (SceneLoader.IsActiveScene(SceneLoader.Scene.HomeBaseScene))
        {
            RewardBonusAugment();
            return;
        }

        DialogueClip nextClip = Managers.D_MAN.EngagedHero.NextDialogueClip;
        if (!SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene))
        {
            DialoguePrompt dp = nextClip as DialoguePrompt;
            if (dp.AetherCells > 0) Managers.U_MAN.CreateAetherCellPopup(dp.AetherCells);
            else Managers.D_MAN.DisplayDialoguePopup();
        }
        else if (nextClip is CombatRewardClip crc)
        {
            int aetherReward = Managers.G_MAN.GetAetherReward((Managers.D_MAN.EngagedHero as EnemyHero).EnemyLevel);
            if (aetherReward > 0) Managers.U_MAN.CreateAetherCellPopup(aetherReward);
            else
            {
                Managers.D_MAN.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
                SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
            }
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }

    public void IgnoreCard_OnClick()
    {
        GetComponent<SoundPlayer>().PlaySound(3);
        Managers.P_MAN.AetherCells += GameManager.IGNORE_CARD_AETHER;
        Managers.U_MAN.DestroyInteractablePopup(gameObject);
        DisableButtons();

        if (SceneLoader.IsActiveScene(SceneLoader.Scene.HomeBaseScene))
        {
            RewardBonusAugment();
            return;
        }

        DialogueClip nextClip = Managers.D_MAN.EngagedHero.NextDialogueClip;
        if (!SceneLoader.IsActiveScene(SceneLoader.Scene.CombatScene))
        {
            DialoguePrompt dp = nextClip as DialoguePrompt;
            if (dp.AetherCells > 0) Managers.U_MAN.CreateAetherCellPopup(dp.AetherCells);
            else Managers.D_MAN.DisplayDialoguePopup();
        }
        else if (Managers.D_MAN.EngagedHero.NextDialogueClip is CombatRewardClip crc)
        {
            int aetherReward = Managers.G_MAN.GetAetherReward((Managers.D_MAN.EngagedHero as EnemyHero).EnemyLevel);
            if (aetherReward > 0) Managers.U_MAN.CreateAetherCellPopup(aetherReward);
            else
            {
                Managers.D_MAN.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
                SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
            }
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }

    public void RedrawCards_OnClick()
    {
        GetComponent<SoundPlayer>().PlaySound(3);

        if (Managers.P_MAN.AetherCells < redrawCost)
        {
            Managers.U_MAN.InsufficientAetherPopup();
            return;
        }

        Managers.P_MAN.AetherCells -= redrawCost;
        CardManager.ChooseCard chooseCardType;
        if (chooseCards[0] is UnitCard) chooseCardType = CardManager.ChooseCard.Unit;
        else chooseCardType = CardManager.ChooseCard.Action;

        Managers.U_MAN.CreateNewCardPopup(null, PopupTitle,
            Managers.CA_MAN.ChooseCards(chooseCardType));
    }

    private void RewardBonusAugment()
    {
        string aetherMagnet = "Aether Magnet";
        if (Managers.P_MAN.GetAugment(aetherMagnet))
        {
            Managers.AN_MAN.TriggerAugment(aetherMagnet);
            Managers.U_MAN.CreateAetherCellPopup(GameManager.AETHER_MAGNET_REWARD);
        }
    }

    private void DisableButtons()
    {
        foreach (GameObject button in addCardButtons)
            button.GetComponent<Button>().interactable = false;
        ignoreCardButton.GetComponent<Button>().interactable = false;
    }
}
