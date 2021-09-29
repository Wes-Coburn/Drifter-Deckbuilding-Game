using UnityEngine;

public class NewCardPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject newCardZone;
    [SerializeField] private GameObject newCardChest;
    [SerializeField] private GameObject addCardButton;
    [SerializeField] private GameObject ignoreCardButton;

    private CardManager caMan;
    private CombatManager coMan;
    private PlayerManager pMan;
    private DialogueManager dMan;
    private UIManager uMan;
    private Card currentCard;

    public Card CurrentCard
    {
        get => currentCard;
        set
        {
            currentCard = value;
            DisplayNewCardChest();
        }
    }

    private void Awake()
    {
        caMan = CardManager.Instance;
        coMan = CombatManager.Instance;
        pMan = PlayerManager.Instance;
        dMan = DialogueManager.Instance;
        uMan = UIManager.Instance;
    }

    private void DisplayNewCardChest()
    {
        gameObject.GetComponent<SoundPlayer>().PlaySound(0);
        newCardChest.SetActive(true);
        addCardButton.SetActive(false);
        ignoreCardButton.SetActive(false);
    }

    public void DisplayNewCard()
    {
        newCardChest.SetActive(false);
        addCardButton.SetActive(true);
        ignoreCardButton.SetActive(true);
        // Card Popup
        GameObject newCard = coMan.ShowCard(CurrentCard, new Vector2(), true);
        CardZoom cz = newCard.GetComponent<CardZoom>();
        newCard.transform.SetParent(newCardZone.transform, false);
        // Description Popup
        cz.CreateDescriptionPopup(new Vector2(-500, 0), 3);
        CardZoom.DescriptionPopup.transform.SetParent(newCardZone.transform, true);
        // Ability Popups
        cz.CreateAbilityPopups(new Vector2(500, 0), 3);
        CardZoom.AbilityPopupBox.transform.SetParent(newCardZone.transform, true);
        // Card Popup
        newCard.transform.SetAsLastSibling();
        // Prevent DestroyZoomObjects() on ZoomAbilityIcon
        CardZoom.CurrentZoomCard = null;
        CardZoom.DescriptionPopup = null;
        CardZoom.AbilityPopupBox = null;
    }

    public void AddCard()
    {
        GetComponent<SoundPlayer>().PlaySound(2);
        caMan.DestroyNewCardPopup();

        DialogueClip nextClip = dMan.EngagedHero.NextDialogueClip;
        if (!coMan.IsInCombat)
        {
            DialoguePrompt dp = nextClip as DialoguePrompt;
            if (dp.AetherCells > 0)
            {
                int newAether = dp.AetherCells;
                int newTotal = newAether + pMan.AetherCells;
                uMan.CreateAetherCellPopup(newAether, newTotal);
            }
            else dMan.DisplayDialoguePopup();
        }
        else if (nextClip is CombatRewardClip crc)
        {
            if (crc.AetherCells > 0)
            {
                int newAether = crc.AetherCells;
                int newTotal = newAether + pMan.AetherCells;
                uMan.CreateAetherCellPopup(newAether, newTotal);
            }
            else
            {
                dMan.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
                SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
                coMan.IsInCombat = false;
            }
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }

    public void IgnoreCard()
    {
        GetComponent<SoundPlayer>().PlaySound(3);
        pMan.PlayerDeckList.Remove(CurrentCard);
        caMan.DestroyNewCardPopup();
        DialogueClip nextClip = dMan.EngagedHero.NextDialogueClip;
        if (!coMan.IsInCombat)
        {
            DialoguePrompt dp = nextClip as DialoguePrompt;
            if (dp.AetherCells > 0)
            {
                int newAether = dp.AetherCells;
                int newTotal = newAether + pMan.AetherCells;
                uMan.CreateAetherCellPopup(newAether, newTotal);
            }
            else dMan.DisplayDialoguePopup();
        }
        else if (dMan.EngagedHero.NextDialogueClip is CombatRewardClip crc)
        {
            if (crc.AetherCells > 0)
            {
                int newAether = crc.AetherCells;
                int newTotal = newAether + pMan.AetherCells;
                uMan.CreateAetherCellPopup(newAether, newTotal);
            }
            else
            {
                dMan.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
                SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
                coMan.IsInCombat = false;
            }
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }
}
