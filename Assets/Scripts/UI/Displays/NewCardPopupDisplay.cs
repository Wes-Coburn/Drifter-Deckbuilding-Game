using UnityEngine;

public class NewCardPopupDisplay : MonoBehaviour
{
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
        GameObject newCard = CardManager.Instance.ShowCard(CurrentCard, true);
        CardZoom cz = newCard.GetComponent<CardZoom>();
        newCard.transform.SetParent(newCardZone.transform, false);
        // Description Popup
        cz.CreateDescriptionPopup(new Vector2(-500, 0), 3);
        CardZoom.DescriptionPopup.transform.SetParent(newCardZone.transform, true);
        // Ability Popups
        cz.CreateAbilityPopups(new Vector2(500, 0), 3);
        CardZoom.AbilityPopupBox.transform.SetParent(newCardZone.transform, true);

        // Prevent DestroyZoomObjects() on ZoomAbilityIcon
        CardZoom.CurrentZoomCard = null;
        CardZoom.DescriptionPopup = null;
        CardZoom.AbilityPopupBox = null;
    }

    public void AddCard()
    {
        if (!CatchScreenDimmer()) return;
        gameObject.GetComponent<SoundPlayer>().PlaySound(2);
        CardManager.Instance.DestroyNewCardPopup();
        DialogueManager.Instance.DisplayDialoguePopup();
    }

    public void IgnoreCard()
    {
        if (!CatchScreenDimmer()) return;
        gameObject.GetComponent<SoundPlayer>().PlaySound(3);
        PlayerManager.Instance.PlayerDeckList.Remove(CurrentCard);
        CardManager.Instance.DestroyNewCardPopup();
        DialogueManager.Instance.DisplayDialoguePopup();
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
