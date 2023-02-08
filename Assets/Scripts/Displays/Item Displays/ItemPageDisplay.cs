using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPageDisplay : MonoBehaviour
{
    [SerializeField] private GameObject pageTitle;
    [SerializeField] private GameObject items;
    [SerializeField] private GameObject itemDescriptionPrefab;
    [SerializeField] private GameObject noItemsTooltip;

    [Header("PROGRESS BAR")]
    [SerializeField] private GameObject progressBar;
    [SerializeField] private GameObject progressFill;
    [SerializeField] private GameObject progressBarText;

    public void SetProgressBar(int currentProgress, int newProgress, bool isReady, bool isFirstDisplay = false)
    {
        string progressText;
        if (isReady) progressText = "DISCOUNT APPLIED!";
        else progressText = newProgress + "/" + GameManager.SHOP_LOYALTY_GOAL + " Items Purchased";
        progressBarText.GetComponent<TextMeshProUGUI>().SetText(progressText);
        if (isFirstDisplay && newProgress < 1) return;
        AnimationManager.Instance.SetProgressBar(currentProgress, newProgress, progressBar, progressFill);
    }
    public void DisplayItems(bool isItemRemoveal, bool playSound)
    {
        if (playSound) AudioManager.Instance.StartStopSound("SFX_CreatePopup1");

        List<HeroItem> currentItems;
        if (isItemRemoveal) currentItems = PlayerManager.Instance.HeroItems;
        else currentItems = GameManager.Instance.ShopItems;

        string title;
        if (isItemRemoveal)
        {
            title = "Sell An Item";
            progressBar.SetActive(false); // TESTING
        }
        else
        {
            title = "Buy An Item";
            bool isReady;
            int progress = GameManager.Instance.ShopLoyalty;
            if (progress == GameManager.SHOP_LOYALTY_GOAL) isReady = true;
            else isReady = false;
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

            ItemDescriptionDisplay idd = description.GetComponentInChildren<ItemDescriptionDisplay>();
            idd.IsItemRemoval = isItemRemoveal; // TESTING
            idd.LoadedItem = item;
        }

        scrollRect.verticalNormalizedPosition = 1;
    }

    public void CloseItemPageButton_OnClick()
    {
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.DialogueScene))
            DialogueManager.Instance.DisplayDialoguePopup();
        UIManager.Instance.DestroyItemPagePopup();
        UIManager.Instance.DestroyRemoveItemPopup();
    }
}

