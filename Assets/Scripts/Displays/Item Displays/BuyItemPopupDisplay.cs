using UnityEngine;
using TMPro;

public class BuyItemPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;
    private PlayerManager pMan;
    private UIManager uMan;
    private GameManager gMan;
    private HeroItem heroItem;

    private string PopupText
    {
        set
        {
            popupText.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    private void Awake()
    {
        pMan = PlayerManager.Instance;
        uMan = UIManager.Instance;
        gMan = GameManager.Instance;
    }

    public HeroItem HeroItem
    {
        set
        {
            heroItem = value;
            string text = "Buy <color=\"yellow\"><b>" + heroItem.ItemName + "</b></color>" +
                " for <color=\"yellow\"><b>" + gMan.GetItemCost(heroItem, out _, false) +
                "</b></color> aether?";
            PopupText = text;
        }
    }
    
    public void ConfirmButton_OnClick()
    {
        pMan.AddItem(heroItem, true);
        pMan.AetherCells -= gMan.GetItemCost(heroItem, out _, false);
        gMan.ShopItems.Remove(heroItem);
        bool isReady = false;
        int previousProgress = gMan.ShopLoyalty;
        if (++gMan.ShopLoyalty == GameManager.SHOP_LOYALTY_GOAL) isReady = true;
        else if (gMan.ShopLoyalty > GameManager.SHOP_LOYALTY_GOAL) gMan.ShopLoyalty = 0;
        uMan.CreateItemPagePopup(false);
        FindObjectOfType<ItemPageDisplay>().SetProgressBar(previousProgress, gMan.ShopLoyalty, isReady);

        AnimationManager.Instance.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress); // TESTING
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyInteractablePopup(gameObject);
}
