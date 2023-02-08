using TMPro;
using UnityEngine;

public class BuyItemPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;
    private HeroItem heroItem;

    private string PopupText
    {
        set
        {
            popupText.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    public HeroItem HeroItem
    {
        set
        {
            heroItem = value;
            string text = "Buy <b><u>" + heroItem.ItemName + "</u></b>" +
                " for <color=\"red\"><b>" + ManagerHandler.G_MAN.GetItemCost(heroItem, out _, false) +
                "</b></color> aether?";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        ManagerHandler.P_MAN.AddItem(heroItem, true);
        ManagerHandler.P_MAN.AetherCells -= ManagerHandler.G_MAN.GetItemCost(heroItem, out _, false);
        ManagerHandler.G_MAN.ShopItems.Remove(heroItem);
        bool isReady = false;
        int previousProgress = ManagerHandler.G_MAN.ShopLoyalty;
        if (++ManagerHandler.G_MAN.ShopLoyalty == GameManager.SHOP_LOYALTY_GOAL) isReady = true;
        else if (ManagerHandler.G_MAN.ShopLoyalty > GameManager.SHOP_LOYALTY_GOAL) ManagerHandler.G_MAN.ShopLoyalty = 0;
        ManagerHandler.U_MAN.CreateItemPagePopup(false);
        FindObjectOfType<ItemPageDisplay>().SetProgressBar(previousProgress, ManagerHandler.G_MAN.ShopLoyalty, isReady);

        AnimationManager.Instance.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress); // TESTING
    }

    public void CancelButton_OnClick() =>
        ManagerHandler.U_MAN.DestroyInteractablePopup(gameObject);
}
