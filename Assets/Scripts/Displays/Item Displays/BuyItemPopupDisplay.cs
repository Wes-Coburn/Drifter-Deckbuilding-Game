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
            string text = $"Buy <b><u>{heroItem.ItemName}</u></b> for " +
                $"{TextFilter.Clrz_red(Managers.G_MAN.GetItemCost(heroItem, out _, false) + "")} aether?";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        Managers.P_MAN.AddItem(heroItem, true);
        Managers.P_MAN.AetherCells -= Managers.G_MAN.GetItemCost(heroItem, out _, false);
        Managers.G_MAN.ShopItems.Remove(heroItem);
        bool isReady = false;
        int previousProgress = Managers.G_MAN.ShopLoyalty;
        if (++Managers.G_MAN.ShopLoyalty == GameManager.SHOP_LOYALTY_GOAL) isReady = true;
        else if (Managers.G_MAN.ShopLoyalty > GameManager.SHOP_LOYALTY_GOAL) Managers.G_MAN.ShopLoyalty = 0;
        Managers.U_MAN.CreateItemPagePopup(false);
        FindObjectOfType<ItemPageDisplay>().SetProgressBar(previousProgress, Managers.G_MAN.ShopLoyalty, isReady);

        Managers.AN_MAN.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress);
    }

    public void CancelButton_OnClick() => Managers.U_MAN.DestroyInteractablePopup(gameObject);
}
