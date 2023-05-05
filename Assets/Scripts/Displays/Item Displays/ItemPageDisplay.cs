using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPageDisplay : MonoBehaviour
{
    [SerializeField] private GameObject pageTitle, items, itemDescriptionPrefab,
        noItemsTooltip, progressBar, progressFill, progressBarText;

    public void SetProgressBar(int currentProgress, int newProgress, bool isReady, bool isFirstDisplay = false)
    {
        string progressText = isReady ? "DISCOUNT APPLIED!" : $"{newProgress}/{GameManager.SHOP_LOYALTY_GOAL} Items Purchased";
        progressBarText.GetComponent<TextMeshProUGUI>().SetText(progressText);
        if (isFirstDisplay && newProgress < 1) return;
        Managers.AN_MAN.SetProgressBar(currentProgress, newProgress, progressBar, progressFill);
    }
    public void DisplayItems(bool isItemRemoveal, bool playSound)
    {
        if (playSound) Managers.AU_MAN.StartStopSound("SFX_CreatePopup1");

        List<HeroItem> currentItems = isItemRemoveal ? Managers.P_MAN.HeroItems : Managers.G_MAN.ShopItems;
        string title;

        if (isItemRemoveal)
        {
            title = "Sell An Item";
            progressBar.SetActive(false);
        }
        else
        {
            title = "Buy An Item";
            int progress = Managers.G_MAN.ShopLoyalty;
            bool isReady = progress == GameManager.SHOP_LOYALTY_GOAL;
            SetProgressBar(0, progress, isReady, true);
        }
        pageTitle.GetComponent<TextMeshProUGUI>().SetText(title);

        if (currentItems.Count < 1)
        {
            noItemsTooltip.SetActive(true);
            return;
        }
        noItemsTooltip.SetActive(false);

        ScrollRect scrollRect = GetComponent<ScrollRect>();

        int rows = Mathf.CeilToInt(currentItems.Count / 4f);
        if (rows < 1) rows = 1;
        float height = rows * 450;

        RectTransform itemRect = scrollRect.content.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(itemRect.rect.width, height);

        foreach (HeroItem item in currentItems)
        {
            GameObject description = Instantiate(itemDescriptionPrefab, items.transform);

            var idd = description.GetComponentInChildren<ItemDescriptionDisplay>();
            idd.IsItemRemoval = isItemRemoveal;
            idd.LoadedItem = item;
        }

        scrollRect.verticalNormalizedPosition = 1;
    }

    public void CloseItemPageButton_OnClick()
    {
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.DialogueScene))
            Managers.D_MAN.DisplayDialoguePopup();

        Managers.U_MAN.DestroyItemPagePopup(true);
        Managers.U_MAN.DestroyInteractablePopup(gameObject);
    }
}

