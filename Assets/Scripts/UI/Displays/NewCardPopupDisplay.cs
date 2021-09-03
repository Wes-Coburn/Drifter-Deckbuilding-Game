using UnityEngine;

public class NewCardPopupDisplay : MonoBehaviour
{
    CardManager caMan;
    CombatManager coMan;
    PlayerManager pMan;
    DialogueManager dMan;

    [SerializeField] private GameObject newCardZone;
    [SerializeField] private GameObject newCardChest;
    [SerializeField] private GameObject addCardButton;
    [SerializeField] private GameObject ignoreCardButton;
    
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
        GameObject newCard = CombatManager.Instance.ShowCard(CurrentCard, true);
        CardZoom cz = newCard.GetComponent<CardZoom>();
        newCard.transform.SetParent(newCardZone.transform, false);
        // Description Popup
        cz.CreateDescriptionPopup(new Vector2(-500, 0), 3);
        CardZoom.DescriptionPopup.transform.SetParent(newCardZone.transform, true);
        // Ability Popups
        cz.CreateAbilityPopups(new Vector2(500, 0), 3);
        CardZoom.AbilityPopupBox.transform.SetParent(newCardZone.transform, true);

        if (CombatManager.Instance.IsInCombat) // TESTING
        {
            if (newCard.TryGetComponent(out ChangeLayer cl)) { }
            else cl = newCard.AddComponent<ChangeLayer>();
            cl.UILayer();

            if (CardZoom.DescriptionPopup.TryGetComponent(out cl)) { }
            else cl = CardZoom.DescriptionPopup.AddComponent<ChangeLayer>();
            cl.UILayer();
        }

        // Prevent DestroyZoomObjects() on ZoomAbilityIcon
        CardZoom.CurrentZoomCard = null;
        CardZoom.DescriptionPopup = null;
        CardZoom.AbilityPopupBox = null;
    }

    public void AddCard()
    {
        if (!CatchScreenDimmer()) return;
        GetComponent<SoundPlayer>().PlaySound(2);
        caMan.DestroyNewCardPopup();
        if (!coMan.IsInCombat) dMan.DisplayDialoguePopup();
        else if (dMan.EngagedHero.NextDialogueClip is CombatRewardClip crc)
        {
            //if (crc.AetherCells != null)
            dMan.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
            SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
            coMan.IsInCombat = false;
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }

    public void IgnoreCard()
    {
        if (!CatchScreenDimmer()) return;
        GetComponent<SoundPlayer>().PlaySound(3);
        pMan.PlayerDeckList.Remove(CurrentCard);
        caMan.DestroyNewCardPopup();
        if (!coMan.IsInCombat) dMan.DisplayDialoguePopup();
        else if (dMan.EngagedHero.NextDialogueClip is CombatRewardClip crc)
        {
            //if (crc.AetherCells != null)
            dMan.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
            SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
            coMan.IsInCombat = false;
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }

    private bool CatchScreenDimmer()
    {
        if (CardZoom.ZoomCardIsCentered)
        {
            UIManager.Instance.DestroyZoomObjects();
            return false;
        }
        else return true;
    }
}
